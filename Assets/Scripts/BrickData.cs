using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BrickData
{
    private static readonly List<Brick> Bricks = new List<Brick>()
    {
        new Brick("1x1", "1x1 Brick", "bricks", "Bricks/Brick_1x1/Brick_1x1_Complete"),
        new Brick("1x2", "1x2 Brick", "bricks", "Bricks/Brick_1x2/Brick_1x2_Complete"),
        new Brick("1x3", "1x3 Brick", "bricks", "Bricks/Brick_1x3/Brick_1x3_Complete"),
        new Brick("1x4", "1x4 Brick", "bricks", "Bricks/Brick_1x4/Brick_1x4_Complete"),
        new Brick("1x6", "1x6 Brick", "bricks", "Bricks/Brick_1x6/Brick_1x6_Complete", 0.8f),
        new Brick("1x8", "1x8 Brick", "bricks", "Bricks/Brick_1x8/Brick_1x8_Complete", 0.8f),
        new Brick("1x10", "1x10 Brick", "bricks", "Bricks/Brick_1x10/Brick_1x10_Complete", 0.65f),
        new Brick("2x2", "2x2 Brick", "bricks", "Bricks/Brick_2x2/Brick_2x2_Complete"),
        new Brick("2x3", "2x3 Brick", "bricks", "Bricks/Brick_2x3/Brick_2x3_Complete"),
        new Brick("4x2", "2x4 Brick", "bricks", "Bricks/Brick_2x4/Brick_2x4_Complete"),
        new Brick("2x6", "2x6 Brick", "bricks", "Bricks/Brick_2x6/Brick_2x6_Complete", 0.8f),
        new Brick("2x8", "2x8 Brick", "bricks", "Bricks/Brick_2x8/Brick_2x8_Complete", 0.65f),
        new Brick("2x2Corner", "2x2 L Brick", "bricks", "Bricks/Brick_2x2_Corner/Brick_2x2_Corner_Complete"),
        new Brick("1x1Tile", "1x1 Tile", "tiles", "Tiles/Tile_1x1/Tile_1x1_Complete"),
        new Brick("1x2Tile", "1x2 Tile", "tiles", "Tiles/Tile_1x2/Tile_1x2_Complete"),
        new Brick("1x3Tile", "1x3 Tile", "tiles", "Tiles/Tile_1x3/Tile_1x3_Complete"),
        new Brick("1x4Tile", "1x4 Tile", "tiles", "Tiles/Tile_1x4/Tile_1x4_Complete"),
        new Brick("1x6Tile", "1x6 Tile", "tiles", "Tiles/Tile_1x6/Tile_1x6_Complete", 0.8f),
        new Brick("1x8Tile", "1x8 Tile", "tiles", "Tiles/Tile_1x8/Tile_1x8_Complete", 0.8f),
        new Brick("1x10Tile", "1x10 Tile", "tiles", "Tiles/Tile_1x10/Tile_1x10_Complete", 0.65f),
        new Brick("2x2Tile", "2x2 Tile", "tiles", "Tiles/Tile_2x2/Tile_2x2_Complete"),
        new Brick("2x3Tile", "2x3 Tile", "tiles", "Tiles/Tile_2x3/Tile_2x3_Complete"),
        new Brick("2x4Tile", "2x4 Tile", "tiles", "Tiles/Tile_2x4/Tile_2x4_Complete"),
        new Brick("2x6Tile", "2x6 Tile", "tiles", "Tiles/Tile_2x6/Tile_2x6_Complete", 0.8f),
        new Brick("2x8Tile", "2x8 Tile", "tiles", "Tiles/Tile_2x8/Tile_2x8_Complete", 0.65f),
        new Brick("2x2CornerTile", "2x2 L Tile", "tiles", "Tiles/Tile_2x2_Corner/Tile_2x2_Corner_Complete"),
        new Brick("1x1RoundTile", "1x1 Round Tile", "tiles", "Tiles/Tile_Round_1x1/Tile_Round_1x1_Complete"),
        new Brick("1x1Plate", "1x1 Plate", "plates", "Plates/Plate_1x1/Plate_1x1_Complete"),
        new Brick("1x2Plate", "1x2 Plate", "plates", "Plates/Plate_1x2/Plate_1x2_Complete"),
        new Brick("1x3Plate", "1x3 Plate", "plates", "Plates/Plate_1x3/Plate_1x3_Complete"),
        new Brick("1x4Plate", "1x4 Plate", "plates", "Plates/Plate_1x4/Plate_1x4_Complete"),
        new Brick("1x6Plate", "1x6 Plate", "plates", "Plates/Plate_1x6/Plate_1x6_Complete", 0.8f),
        new Brick("1x8Plate", "1x8 Plate", "plates", "Plates/Plate_1x8/Plate_1x8_Complete", 0.8f),
        new Brick("1x10Plate", "1x10 Plate", "plates", "Plates/Plate_1x10/Plate_1x10_Complete", 0.65f),
        new Brick("2x2Plate", "2x2 Plate", "plates", "Plates/Plate_2x2/Plate_2x2_Complete"),
        new Brick("2x3Plate", "2x3 Plate", "plates", "Plates/Plate_2x3/Plate_2x3_Complete"),
        new Brick("2x4Plate", "2x4 Plate", "plates", "Plates/Plate_2x4/Plate_2x4_Complete"),
        new Brick("2x6Plate", "2x6 Plate", "plates", "Plates/Plate_2x6/Plate_2x6_Complete", 0.8f),
        new Brick("2x8Plate", "2x8 Plate", "plates", "Plates/Plate_2x8/Plate_2x8_Complete", 0.65f),
        new Brick("2x2CornerPlate", "2x2 L Plate", "plates", "Plates/Plate_2x2_Corner/Plate_2x2_Corner_Complete"),
        new Brick("1x2Plate1Stud", "1x2 Plate (1)", "plates", "Plates/Plate_1x2_1Stud/Plate_1x2_1Stud_Complete"),
        new Brick("2x2Plate1Stud", "2x2 Plate (1)", "plates", "Plates/Plate_2x2_1Stud/Plate_2x2_1Stud_Complete"),
        new Brick("1x1RoundPlate", "1x1 Round Plate", "plates", "Plates/Plate_Round_1x1/Plate_Round_1x1_Complete"),
        new Brick("1x2Slope", "1x2 45° Slope", "slopes", "Slopes/Slope_1x2/Slope_1x2_Complete"),
        new Brick("1x3Slope", "1x3 33° Slope", "slopes", "Slopes/Slope_1x3/Slope_1x3_Complete"),
        new Brick("1x4Slope", "1x4 18° Slope", "slopes", "Slopes/Slope_1x4/Slope_1x4_Complete"),
        new Brick("2x2Slope", "2x2 45° Slope", "slopes", "Slopes/Slope_2x2/Slope_2x2_Complete"),
        new Brick("2x2CornerSlope", "2x2 45° Corner", "slopes", "Slopes/Slope_Corner_2x2/Slope_Corner_2x2_Complete"),
        new Brick("3x3CornerSlope", "3x3 33° Corner", "slopes", "Slopes/Slope_Corner_3x3/Slope_Corner_3x3_Complete"),
        new Brick("4x4CornerSlope", "4x4 18° Corner", "slopes", "Slopes/Slope_Corner_4x4/Slope_Corner_4x4_Complete"),
        new Brick("1x2SlopeInverted", "1x2 45° Slope (I)", "slopes", "Slopes/Slope_1x2_Inverted/Slope_1x2_Inverted_Complete"),
        new Brick("1x1-2_3Slope", "1x1 2/3 Slope", "slopes", "Slopes/Slope_1x1x2-3/Slope_1x1x2-3_Complete", 1f, new Vector3(0, -90f, 0)),
        new Brick("1x4-2_3Slope", "1x4 2/3 Slope", "slopes", "Slopes/Slope_1x4x2-3/Slope_1x4x2-3_Complete", 1f, new Vector3(0, -90f, 0)),
        new Brick("1x2Window", "1x2 Window", "misc", "Windows/Window_1x2/Window_1x2_Complete"),
        new Brick("FlatPlant", "Flat Plant", "misc", "Plants/Plant_Green_Flat", 1.3f),
        new Brick("ThreeLeafedPlant", "3-Leafed Plant", "misc", "Plants/Plant_Green_MediumHeight", 1.3f),
        new Brick("1x1_1FaceSideStud", "1x1 (1 face)", "side_studs", "SideStudBricks/Brick_1x1_1FaceSideStud/Brick_1x1_1FaceSideStud_Complete", 1f, new Vector3(0, 180f, 0)),
        new Brick("1x2_1FaceSideStud", "1x2 (1 face)", "side_studs", "SideStudBricks/Brick_1x2_1FaceSideStud/Brick_1x2_1FaceSideStud_Complete",1f, new Vector3(0, 180f, 0)),
        new Brick("1x4_1FaceSideStud", "1x4 (1 face)", "side_studs", "SideStudBricks/Brick_1x4_1FaceSideStud/Brick_1x4_1FaceSideStud_Complete", 1f, new Vector3(0, 180f, 0)),
        new Brick("1x2Masonry", "1x2 Masonry", "misc", "Bricks_Masonry/Brick_1x2_Masonry/Brick_1x2_Masonry_Complete"),
        new Brick("1x4Masonry", "1x4 Masonry", "misc", "Bricks_Masonry/Brick_1x4_Masonry/Brick_1x4_Masonry_Complete"),
        new Brick("1x3Arch", "1x3 Arch", "misc", "Arches/Brick_1x3_Arch/Brick_1x3_Arch_Complete"),
        new Brick("1x4Arch", "1x4 Arch", "misc", "Arches/Brick_1x4_Arch/Brick_1x4_Arch_Complete"),
        new Brick("1x6Arch", "1x6 Arch", "misc", "Arches/Brick_1x6_Arch/Brick_1x6_Arch_Complete", 1f, Vector3.zero, new Vector3(0, -0.03f, 0)),
        new Brick("1x1Pyramid", "1x1 Pyramid", "misc", "Pyramids/Brick_1x1_Pyramid/Brick_1x1_Pyramid_Complete", 1.5f),

        //Slope_1x1x2-3_Complete
        // new Brick("1x1_4FaceSideStud", "1x1 (4 side studs)", "side_studs", "SideStudBricks/Brick_1x1_4FaceSideStud/Brick_1x1_4FaceSideStud_Complete"),
        // new Brick("Bob", "Bob", "bricks", "Misc/Bob"),
    };

    public static List<Brick> AllBricks()
    {
        List<Brick> returnedBricks = new List<Brick>(Bricks);
        SortResults(returnedBricks);

        return returnedBricks;
    }

    public static List<Brick> BricksForCategory(string category)
    {
        List<Brick> returnedBricks = new List<Brick>(Bricks);
        return returnedBricks.Where(b => b.Category == category).ToList();
    }

    public static Brick BrickByPrefabName(string prefabName)
    {
        return Bricks.Find(b => b.PrefabName == prefabName);
    }

    public static Brick BrickByCompleteMeshName(string meshName)
    {
        Debug.Log($"Searching for: {meshName}");
        return Bricks.Find(b => b.CompleteMeshPath.Contains(meshName));
    }

    private static void SortResults(List<Brick> bricks)
    {
        bricks.Sort(BrickCompare);
    }

    private static int BrickCompare(Brick a, Brick b)
    {
        return string.CompareOrdinal(a.Category, b.Category);
    }

    public readonly struct Brick
    {
        public Brick(string prefabName, string displayName, string category, string completeMeshPath,
            float uiScaleModifier = 1f, Vector3? rotation = null, Vector3? handSpawnerPositionOffset = null)
        {
            PrefabName = prefabName;
            DisplayName = displayName;
            Category = category;
            CompleteMeshPath = completeMeshPath;
            Mesh = Resources.Load<Mesh>($"BrickModels/{completeMeshPath}");
            UIScaleModifier = uiScaleModifier;
            Rotation = rotation ?? Vector3.zero;
            HandSpawnerPositionOffset = handSpawnerPositionOffset ?? Vector3.zero;
        }

        public readonly string PrefabName;
        public readonly string DisplayName;
        public readonly string CompleteMeshPath;
        public readonly string Category;
        public readonly Mesh Mesh;
        public readonly float UIScaleModifier;
        public readonly Vector3 Rotation;
        public readonly Vector3 HandSpawnerPositionOffset;
    }

    public class LocalBrickData {
        public CustomQuaternion rot;
        public CustomVec3 pos;
        public string type;
        public int color;
    }

    public class CustomVec3 {
        public float x;
        public float y;
        public float z;

        public static CustomVec3 From(Vector3 vec3)
        {
            return new CustomVec3 {
                x = vec3.x,
                y = vec3.y,
                z = vec3.z,
            };
        }

        public static Vector3 To(CustomVec3 vec3)
        {
            return new Vector3
            (
                vec3.x,
                vec3.y,
                vec3.z
            );
        }
    }

    public class CustomQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static CustomQuaternion From(Quaternion quat)
        {
            return new CustomQuaternion
            {
                x = quat.x,
                y = quat.y,
                z = quat.z,
                w = quat.w,
            };
        }

        public static Quaternion To(CustomQuaternion quat)
        {
            return new Quaternion
            (
                quat.x,
                quat.y,
                quat.z,
                quat.w
            );
        }
    }
}