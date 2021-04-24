using System.Reflection.Emit;

namespace YantraJS.Generator
{
    public static class ILGeneratorExtensions
    {

        public static void EmitConstant(this ILGenerator il, float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
        }

        public static void EmitConstant(this ILGenerator il, double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
        }

        public static void EmitConstant(this ILGenerator il, bool value)
        {
            il.Emit(value ? OpCodes.Ldc_I4_1: OpCodes.Ldc_I4_0);
        }


        public static void EmitConstant(this ILGenerator il, string @string)
        {
            il.Emit(OpCodes.Ldstr, @string);
        }

        public static void EmitConstant(this ILGenerator il, int i)
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
            il.Emit(OpCodes.Ldc_I4_S, i);
            return;
        }
    }
}
