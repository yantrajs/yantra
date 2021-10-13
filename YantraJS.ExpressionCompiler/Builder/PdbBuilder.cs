using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace YantraJS.Builder
{
    //public class PdbBuilder
    //{
    //    private int _id = 0;

    //    private MetadataBuilder builder;

    //    private PortablePdbBuilder portable;

    //    public int Next => ++_id;

    //    Guid s_guidSha1 = unchecked(new Guid((int)0xff1816ec, (short)0xaa5e, 0x4d10, 0x87, 0xf7, 0x6f, 0x49, 0x63, 0x83, 0x34, 0x60));
    //    private readonly DocumentHandle document;

    //    public PdbBuilder(string filePath, string content)
    //    {
    //        builder = new MetadataBuilder();



    //        var documentName = builder.GetOrAddBlobUTF16(filePath);
    //        var guid = builder.GetOrAddGuid(new Guid("3A12D0B6-C26C-11D0-B442-00A0244A1DD2"));

    //        var sha1 = System.Security.Cryptography.SHA1.Create();
    //        var hashGuid = builder.GetOrAddGuid(s_guidSha1);

    //        var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(content));

    //        var hashHandle = builder.GetOrAddBlob(hash);

    //        this.document = builder.AddDocument(documentName, hashGuid, hashHandle, guid);
    //    }

    //    public void Save(System.Reflection.Emit.MethodBuilder mb, ImmutableArray<SequencePoint> sequencePoints)
    //    {

            

    //        var sequencePointsHandle = SaveToBlob(sequencePoints);

    //        var mdh = builder.AddMethodDebugInformation(document, sequencePointsHandle);

    //        builder.AddLocalScope(mdh,)
    //    }

    //    private BlobHandle SaveToBlob(ImmutableArray<SequencePoint> sequencePoints)
    //    {
    //        if (sequencePoints.Length == 0)
    //        {
    //            return default(BlobHandle);
    //        }

    //        var writer = new BlobBuilder();

    //        int previousNonHiddenStartLine = -1;
    //        int previousNonHiddenStartColumn = -1;

    //        // header:
    //        writer.WriteCompressedInteger(MetadataTokens.GetRowNumber(localSignatureHandleOpt));

    //        var previousDocument = TryGetSingleDocument(sequencePoints);
    //        singleDocumentHandle = (previousDocument != null) ? GetOrAddDocument(previousDocument, documentIndex) : default(DocumentHandle);

    //        for (int i = 0; i < sequencePoints.Length; i++)
    //        {
    //            var currentDocument = sequencePoints[i].Document;
    //            if (previousDocument != currentDocument)
    //            {
    //                var documentHandle = GetOrAddDocument(currentDocument, documentIndex);

    //                // optional document in header or document record:
    //                if (previousDocument != null)
    //                {
    //                    writer.WriteCompressedInteger(0);
    //                }

    //                writer.WriteCompressedInteger(MetadataTokens.GetRowNumber(documentHandle));
    //                previousDocument = currentDocument;
    //            }

    //            // delta IL offset:
    //            if (i > 0)
    //            {
    //                writer.WriteCompressedInteger(sequencePoints[i].Offset - sequencePoints[i - 1].Offset);
    //            }
    //            else
    //            {
    //                writer.WriteCompressedInteger(sequencePoints[i].Offset);
    //            }

    //            if (sequencePoints[i].IsHidden)
    //            {
    //                writer.WriteInt16(0);
    //                continue;
    //            }

    //            // Delta Lines & Columns:
    //            SerializeDeltaLinesAndColumns(writer, sequencePoints[i]);

    //            // delta Start Lines & Columns:
    //            if (previousNonHiddenStartLine < 0)
    //            {
    //                Debug.Assert(previousNonHiddenStartColumn < 0);
    //                writer.WriteCompressedInteger(sequencePoints[i].StartLine);
    //                writer.WriteCompressedInteger(sequencePoints[i].StartColumn);
    //            }
    //            else
    //            {
    //                writer.WriteCompressedSignedInteger(sequencePoints[i].StartLine - previousNonHiddenStartLine);
    //                writer.WriteCompressedSignedInteger(sequencePoints[i].StartColumn - previousNonHiddenStartColumn);
    //            }

    //            previousNonHiddenStartLine = sequencePoints[i].StartLine;
    //            previousNonHiddenStartColumn = sequencePoints[i].StartColumn;
    //        }

    //        return _debugMetadataOpt.GetOrAddBlob(writer);
    //    }
    //}
}
