namespace JSON;

public struct Task
{
    public int Qualitative { get; set; }

    public int YearsOld { get; set; }

    public List<float> Time { get; set; }

    public List<float>[] Channels { get; set; }

    public Task()
    {
        Time = new List<float>();
        Channels = new List<float>[8];
        for (int i = 0; i < Channels.Length; i++)
        {
            Channels[i] = new List<float>();
        }
    }
}