using System;
using System.Collections.Generic;

public struct ManiaNote
{
    public int lane;
    public int startMs;
    public int endMs;

    public bool IsHold => endMs >= 0;
}

[Serializable]
public class ManiaChart
{
    public int keyCount;
    public List<ManiaNote> notes = new List<ManiaNote>();
}

public static class OsuManiaParser
{
    public static ManiaChart Parse(string osuText)
    {
        if (string.IsNullOrEmpty(osuText))
            throw new ArgumentException("osuText is empty");

        //Mode 3 is mania, so this is just checking that mania exists in the osu map
        if(!osuText.Contains("Mode: 3"))
            throw new Exception("Not an Osu!mania map (Mode: 3 not found).");

        var chart = new ManiaChart();

        bool inDifficulty = false;
        bool inHitObjects = false; 

        var lines = osuText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//")) continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                inDifficulty = line.Equals("[Difficulty]", StringComparison.OrdinalIgnoreCase);
                inHitObjects = line.Equals("[HitObjects]", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (inDifficulty)
            {
                if(line.StartsWith("CircleSize:", StringComparison.OrdinalIgnoreCase))
                    chart.keyCount = int.Parse(line.Split(':')[1].Trim());
            }
            else if (inHitObjects)
            {
                var parts = line.Split(',');
                if (parts.Length < 5) 
                    continue;

                int x = int.Parse(parts[0]);
                int timeMs = int.Parse(parts[2]);
                int type = int.Parse(parts[3]);

                if (chart.keyCount <= 0)
                    continue; //Osz files store circle size before hit objects

                int lane = Math.Clamp((int)Math.Floor(x * chart.keyCount / 512.0), 0, chart.keyCount - 1);

                bool isHold = (type & 128) != 0;
                int endMs = -1;

                if (isHold && parts.Length >= 6)
                {
                    // parts[5] starts with endTime before ':'
                    string extras = parts[5];
                    int colon = extras.IndexOf(':');
                    string endStr = colon >= 0 ? extras.Substring(0, colon) : extras;
                    endMs = int.Parse(endStr);
                }

                chart.notes.Add(new ManiaNote
                {
                    lane = lane,
                    startMs = timeMs,
                    endMs = endMs
                });
            }
        }

        if (chart.keyCount <= 0)
            throw new Exception("Could not read key count (CircleSize) from [Difficulty].");

        chart.notes.Sort((a, b) => a.startMs.CompareTo(b.startMs));
        return chart;
    }

    public static string GetAudioFilename(string osuText)
    {
        bool inGeneral = false;
        var lines = osuText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//")) continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                inGeneral = line.Equals("[General]", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (!inGeneral) continue;

            if (line.StartsWith("AudioFilename:", StringComparison.OrdinalIgnoreCase))
                return line.Split(':', 2)[1].Trim();
        }

        return null;
    }
}
