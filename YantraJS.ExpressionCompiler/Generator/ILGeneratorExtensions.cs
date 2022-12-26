using System;
using System.Reflection;
using System.Reflection.Emit;
using YantraJS.Core;

namespace YantraJS.Generator
{
    public static class ILGeneratorExtensions
    {

        ///// <summary>
        ///// Emits instruction with _s if index is less or equal to 32767
        ///// </summary>
        ///// <param name="il"></param>
        ///// <param name="opCode_S"></param>
        ///// <param name="opCode"></param>
        ///// <param name="index"></param>
        //public static void Emit(this ILWriter il,OpCode opCode_S, OpCode opCode, int index)
        //{
        //    if (index <= 32767)
        //    {
        //        il.Emit(opCode_S, (short)index);
        //        return;
        //    }
        //    il.Emit(opCode, index);
        //}

        public static void EmitConstant(this ILWriter il, DateTime d)
        {
            throw new NotSupportedException();
        }

        public static void EmitConstant(this ILWriter il,object value, Type valueType = null)
        {
            if(value == null)
            {
                il.Emit(OpCodes.Ldnull);
                return;
            }

            valueType = valueType ?? value.GetType();

            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    il.EmitConstant((bool)value);
                    return;
                case TypeCode.Byte:
                    il.EmitConstant((byte)value);
                    return;
                case TypeCode.Char:
                    il.EmitConstant((short)value);
                    return;
                case TypeCode.DateTime:
                    il.EmitConstant((DateTime)value);
                    return;
                case TypeCode.Decimal:
                    il.EmitConstant((decimal)value);
                    return;
                case TypeCode.Double:
                    il.EmitConstant((double)value);
                    return;
                case TypeCode.Int16:
                    il.EmitConstant((short)value);
                    return;
                case TypeCode.Int32:
                    il.EmitConstant((int)value);
                    return;
                case TypeCode.Int64:
                    il.EmitConstant((long)value);
                    return;
                case TypeCode.SByte:
                    il.EmitConstant((sbyte)value);
                    return;
                case TypeCode.Single:
                    il.EmitConstant((float)value);
                    return;
                case TypeCode.String:
                    il.EmitConstant((string)value);
                    return;
                case TypeCode.UInt16:
                    il.EmitConstant((ushort)value);
                    return;
                case TypeCode.UInt32:
                    il.EmitConstant((uint)value);
                    return;
                case TypeCode.UInt64:
                    il.EmitConstant((ulong)value);
                    return;
            }


            switch (value)
            {
                case string @string:
                    il.EmitConstant(@string);
                    return;
                case int @int:
                    il.EmitConstant(@int);
                    return;
                case bool b:
                    il.EmitConstant(b);
                    return;
                case float f:
                    il.EmitConstant(f);
                    return;
                case double d:
                    il.EmitConstant(d);
                    return;
                case decimal dm:
                    il.EmitConstant(dm);
                    return;
                case Type type:
                    il.Emit(OpCodes.Ldtoken, type);
                    il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
                    return;
                case MethodInfo method:
                    il.Emit(OpCodes.Ldftn, method);
                    return;
                //case Delegate d:
                //    il.Emit(OpCodes.Ldftn, d.Method);
                //    return;
            }
            //if (typeof(Delegate).IsAssignableFrom(value.GetType()))
            //{
            //    il.Emit(OpCodes.Ldftn, ((Delegate)value).Method);
            //    return;
            //}
            throw new NotSupportedException($"Constant of type  {value.GetType()} not supported, you must use a factory to create value of specified type");

        }

        public static void EmitConstant(this ILWriter il, MethodInfo method)
        {
            il.Emit(OpCodes.Ldftn, method);
        }

        public static void EmitConstant(this ILWriter il, Type type)
        {
            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
        }

        public static void EmitConstant(this ILWriter il,decimal value)
        {
            // var bits = new int[4];
            var bits = decimal.GetBits(value);
            Type type = typeof(decimal);

            int scale = (bits[3] & int.MaxValue) >> 16;
            if (scale == 0)
            {
                if (int.MinValue <= value)
                {
                    if (value <= int.MaxValue)
                    {
                        int intValue = decimal.ToInt32(value);
                        switch (intValue)
                        {
                            case -1:
                                il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.MinusOne)));
                                return;
                            case 0:
                                il.Emit(OpCodes.Initobj, typeof(decimal));
                                return;
                            case 1:
                                il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.One)));
                                return;
                            default:
                                il.EmitConstant(intValue);
                                il.EmitNew(type.GetConstructor(typeof(int)));
                                return;
                        }
                    }

                    if (value <= uint.MaxValue)
                    {
                        il.EmitConstant(decimal.ToUInt32(value));
                        il.EmitNew(type.GetConstructor(typeof(uint)));
                        return;
                    }
                }

                if (long.MinValue <= value)
                {
                    if (value <= long.MaxValue)
                    {
                        il.EmitConstant(decimal.ToInt64(value));
                        il.EmitNew(type.GetConstructor(typeof(long)));
                        return;
                    }

                    if (value <= ulong.MaxValue)
                    {
                        il.EmitConstant(decimal.ToUInt64(value));
                        il.EmitNew(type.GetConstructor(typeof(ulong)));
                        return;
                    }

                    if (value == decimal.MaxValue)
                    {
                        il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.MaxValue)));
                        return;
                    }
                }
                else if (value == decimal.MinValue)
                {
                    il.Emit(OpCodes.Ldsfld, type.GetField(nameof(decimal.MinValue)));
                    return;
                }
            }

            il.EmitConstant(bits[0]);
            il.EmitConstant(bits[1]);
            il.EmitConstant(bits[2]);
            il.EmitConstant((bits[3] & 0x80000000) != 0);
            il.EmitConstant(unchecked((byte)scale));
            // il.EmitNew(Decimal_Ctor_Int32_Int32_Int32_Bool_Byte);
            il.EmitNew(type.GetConstructor(typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte)));
        }

        public static void EmitNew(this ILWriter il,ConstructorInfo c)
        {
            il.Emit(OpCodes.Newobj, c);
        }

        public static void EmitConstant(this ILWriter il,float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
        }

        public static void EmitConstant(this ILWriter il,double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
        }

        public static void EmitConstant(this ILWriter il,bool value)
        {
            il.Emit(value ? OpCodes.Ldc_I4_1: OpCodes.Ldc_I4_0);
        }

        public static void EmitConstant(this ILWriter il,uint i)
        {
            il.EmitConstant(unchecked((int)i));
        }



        public static void EmitConstant(this ILWriter il,ulong i)
        {
            il.EmitConstant(unchecked((long)i));
        }

        public static void EmitConstant(this ILWriter il,long i)
        {
            il.Emit(OpCodes.Ldc_I8, i);
        }

        public static void EmitSaveLocal(this ILWriter il,int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Stloc_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Stloc_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Stloc_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Stloc_3);
                    return;
            }
            if (index <= 255)
            {
                il.Emit(OpCodes.Stloc_S, (byte)index);
                return;
            }
            il.Emit(OpCodes.Stloc, index);
        }

        public static void EmitSaveArg(this ILWriter il, int index)
        {
            if(index <= 255)
            {
                il.Emit(OpCodes.Starg_S, (byte)index);
                return;
            }
            il.Emit(OpCodes.Starg, index);
        }

        public static void EmitLoadArg(this ILWriter il,int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    return;
            }
            if(index <= 255)
            {
                il.Emit(OpCodes.Ldarg_S, (byte)index);
                return;
            }
            il.Emit(OpCodes.Ldarg, index);

        }

        public static void EmitLoadArgAddress(this ILWriter il,int index)
        {
            if(index<=255)
            {
                il.Emit(OpCodes.Ldarga_S, (byte)index);
                return;
            }
            il.Emit(OpCodes.Ldarga, index);
        }


        public static void EmitLoadLocal(this ILWriter il,int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldloc_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldloc_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldloc_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldloc_3);
                    return;
            }
            if (index <= 255)
            {
                il.Emit(OpCodes.Ldloc_S, (byte)index);
                return;
            }
            il.Emit(OpCodes.Ldloc, index);
        }

        public static void  EmitCall(this ILWriter il,MethodInfo method)
        {
            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Callvirt, method);
                return;
            }
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitLoadLocalAddress(this ILWriter il,int index)
        {
            if (index <= 255)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)index);
                return;
            }
            il.Emit(OpCodes.Ldloca, index);
        }


        public static void EmitConstant(this ILWriter il,string @string)
        {
            il.Emit(OpCodes.Ldstr, @string);
        }

        //public static void EmitConstant(this ILWriter il, byte b)
        //{

        //}

        public static void EmitConstant(this ILWriter il,int i)
        {
            switch (i)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            //if (sbyte.MinValue > i && i < sbyte.MaxValue) {
            //    il.Emit(OpCodes.Ldc_I4_S, (sbyte)i);
            //    return;
            //}
            il.Emit(OpCodes.Ldc_I4, i);
            return;
        }
    }
}
