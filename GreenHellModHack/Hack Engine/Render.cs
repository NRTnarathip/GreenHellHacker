using UnityEngine;
namespace HackEngine
{
    public static class Render
    {
        public static bool CheckPointOutOfView(Vector3 point)
        {
            var view = Camera.main.WorldToViewportPoint(point);
            return view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1 || view.z < 0;
        }
        public static void DrawString(Vector3 worldPos, string text, int fontSize, Color color)
        {
            if (CheckPointOutOfView(worldPos)) return;
            // Create a GUIStyle to set the font size
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = color;

            Vector2 pos = Camera.main.WorldToScreenPoint(worldPos);
            //convert coordinate gui system
            pos.y = Screen.height - pos.y;

            Vector2 labelSize = style.CalcSize(new GUIContent(text));
            var labelSizeHalf = labelSize / 2;
            Rect labelRect = new Rect(
                pos.x - labelSizeHalf.x,
                pos.y - labelSizeHalf.y,
                labelSize.x, labelSize.y);

            GUI.Label(labelRect, text, style);
        }

        public static Texture2D lineTex;
        static Matrix4x4 matrix = GUI.matrix;
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            if (!lineTex)
                lineTex = new Texture2D(1, 1);

            Color color2 = GUI.color;
            GUI.color = color;
            var screenHeight = Screen.height;

            //invert coordinate GUI Top->Down Convert new Down->Top
            pointA.y = screenHeight - pointA.y;
            pointB.y = screenHeight - pointB.y;

            float num = Vector3.Angle(pointB - pointA, Vector2.right);

            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        public static void DrawBoxOutline(Vector2 Point, float width, float height, Color color, float thickness)
        {
            DrawLine(Point, new Vector2(Point.x + width, Point.y), color, thickness);
            DrawLine(Point, new Vector2(Point.x, Point.y + height), color, thickness);
            DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x + width, Point.y), color, thickness);
            DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x, Point.y + height), color, thickness);
        }

        public static void DrawESPBox(Vector3 pos, Vector2 bodyScale, Color color, float lineWidth)
        {
            if (CheckPointOutOfView(pos)) return;

            if (pos.z == 0f) pos.z = 0.0001f;
            var player2D = Camera.main.WorldToScreenPoint(pos);
            if (player2D.z < 0) return;

            Camera camera = Camera.main;
            var head2D = Camera.main.WorldToScreenPoint(pos + camera.transform.right + camera.transform.up);
            var bodySize2D = head2D - player2D;
            bodySize2D.x *= bodyScale.x;
            bodySize2D.y *= bodyScale.y;
            var bodySize2DHalf = bodySize2D / 2f;

            DrawBoxOutline(player2D - bodySize2DHalf,
                bodySize2D.x,
                bodySize2D.y,
                color, lineWidth);
        }
    }

}
