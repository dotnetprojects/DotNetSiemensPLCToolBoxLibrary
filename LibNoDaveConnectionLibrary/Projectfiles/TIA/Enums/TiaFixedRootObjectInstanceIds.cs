using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums
{
    public enum TiaFixedRootObjectInstanceIds : long
    {
        MetaId = 1L,
        StorageMetaMappingTableId = 2L,
        ObjectInstanceAlloctionTableId = 3L,
        ClusterAllocationTableId = 5L,
        SystemAllocationTableId = 6L,
        ExpandoMappingTableId = 7L,
        ClusterMappingTableId = 8L,
        StorageInformationId = 9L,
        PropertySetMappingTableId = 10L,
        BlobMappingTableId = 11L,
        ProjectAnnotationTableId = 12L,
        StorageStatisticsId = 14L,
        SessionInfoId = 15L,
        StorageVersionId = 0x10L,
        RootObjectCollectionId = 20L,
        ClusterCollectionId = 0x15L,
        LocalizatonTableId = 0x16L,
        EndBlockId = 30L,
        TeminatorId = 0x1fL,
        DefectEntryId = 100L,
    }
}
