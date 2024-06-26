using SWFServer.Data;
using SWFServer.Data.Entities;
using UnityEngine;

public static class Util
{
    public static Vector3 ToVector3(Vector2 p)
    {
        return new Vector3(p.x, p.y, 0);
    }

    public static Vector2 ToVector2(Vector2f p)
    {
        return new Vector2(p.x, p.y);
    }

    public static Vector3 ToVector3(Vector2f p)
    {
        return new Vector3(p.x, 0, p.y);
    }

    public static Vector2 IntersectLineRect(Vector2 a, Vector2 b, Rect rect)
    {
        // ѕолучим параметрическое уравнение линии AB
        float dx = b.x - a.x;
        float dy = b.y - a.y;

        // ѕроверим пересечение со сторонами пр€моугольника
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;

        // ѕроверка и вычисление пересечени€ с вертикальными сторонами
        if (dx != 0)
        {
            float x1 = (xMin - a.x) / dx;
            float x2 = (xMax - a.x) / dx;
            float y1 = a.y + x1 * dy;
            float y2 = a.y + x2 * dy;
            if (y1 >= yMin && y1 <= yMax && x1 >= 0 && x1 <= 1)
                return new Vector2(xMin, y1);
            if (y2 >= yMin && y2 <= yMax && x2 >= 0 && x2 <= 1)
                return new Vector2(xMax, y2);
        }

        // ѕроверка и вычисление пересечени€ с горизонтальными сторонами
        if (dy != 0)
        {
            float y1 = (yMin - a.y) / dy;
            float y2 = (yMax - a.y) / dy;
            float x1 = a.x + y1 * dx;
            float x2 = a.x + y2 * dx;
            if (x1 >= xMin && x1 <= xMax && y1 >= 0 && y1 <= 1)
                return new Vector2(x1, yMin);
            if (x2 >= xMin && x2 <= xMax && y2 >= 0 && y2 <= 1)
                return new Vector2(x2, yMax);
        }

        // “очка пересечени€ не найдена
        return b;
    }

    public static Rect ShrinkRect(Rect rect, float amount)
    {
        Rect newRect = new Rect(rect);
        newRect.x = rect.x + amount;
        newRect.y = rect.y + amount;
        newRect.width -= 2 * amount;
        newRect.height -= 2 * amount;
        return newRect;
    }

    public static string ToTimeStringMS(int sec)
    {
        int m = (int)(sec / 60.0f);
        int s = (int)sec % 60;
        return m + ":" + s.ToString("00");
    }

    public static string ToTimeStringHM(int sec)
    {
        int h = sec / 3600;
        int m = (sec - h * 3600) / 60;
        return h + ":" + m.ToString("00");
    }

    public static string ToTimeStringHMS(int sec)
    {
        int h = sec / 3600;
        int m = (sec % 3600) / 60;
        int s = sec % 60;
        if (h > 0)
            return h + ":" + m.ToString("00") + ":" + s.ToString("00");
        return m + ":" + s.ToString("00");
    }

    public static string ToTimeStringDHM(int sec)
    {
        int d = sec / 86400;
        int h = (sec % 86400) / 3600;
        int m = (sec % 3600) / 60;
        if (d > 0)
            return d + ":" + h.ToString("00") + ":" + m.ToString("00");
        return h + ":" + m.ToString("00");
    }

    public static int CalcUnitItems(bool isTask, ushort itemId)
    {
        if (isTask)
        {
            var taskId = Data.Instance.Unit.Unit.TaskId;
            var task = Data.Instance.tasks.Find(f => f.Id == taskId);

            var info = Info.EntityInfo[task.ItemId];

            var res = info.craft.items.Find(f => f.id == Info.EntityInfo[itemId].name);

            return res.count * task.Count;
        }

        var list = Data.Instance.Unit.Entities.Entities.FindAll(f => f.Id == itemId);
        int c = 0;

        for (int i = 0; i < list.Count; i++)
        {
            c += list[i].Count.Value;
        }

        return c;
    }

}
