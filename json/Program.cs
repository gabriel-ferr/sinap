using System.Text.Json;

namespace JSON;

public static class Program
{
    static List<string> qualitativeFiles = new ();
    static List<string> qualitativeFolder = new ();
    static List<string> quantitativeFiles = new ();
    static List<string> quantitativeFolder = new ();

    static List<Qualitative> qualitatives = new ();

    static List<List<Task>> resultList = new ();

    static void Parse(string file)
    {
        StreamReader reader = new (file);
        string content = reader.ReadToEnd();
        reader.Close();

        string[] rawBlocks = content.Replace("{", "&").Replace("}", "&").Replace("\'", "").Split('&');

        List<string> blocks = new();
        foreach(string block in rawBlocks)
        {
            if (string.IsNullOrEmpty(block) || string.IsNullOrWhiteSpace(block)) continue;
            blocks.Add(block);
        }

        int id = Convert.ToInt32(blocks[0].Split(',')[0].Split(':')[1].Replace("\"", ""));

        List<Task> tasks = new ();

        for (int i = 2; i < blocks.Count; i += 2)
        {
            string[] data = blocks[i].Split(':')[10].Replace("\",", "\"&").Split('&');

            Dictionary<int, List<float>> channels = new ();
            int channelIndex = 1;
            foreach (string dataContent in data)
            {
                List<float> channel = new ();
                string[] _raw = dataContent.Replace("\"", "").Replace(" ", "").Replace("]", "").Replace("[", "").Replace("\n", "").Split('_');
                foreach (string _content in _raw)
                    channel.Add(Convert.ToSingle(_content.Replace('.', ',')));
                channels.Add(channelIndex, channel);
                channelIndex++;
            }

            int rows = channels[1].Count;
            try {
                foreach (List<float> channel in channels.Values)
                {
                    if (rows != channel.Count)
                        throw new Exception($"A referência de linhas para os canais da tarefa {(i / 2) - 1} na coleta {id} não é válida.");
                }
            } catch (Exception e) { Console.WriteLine(e.Message); continue; }

            List<float> time = new ();
            float last = 0;
            time.Add(0);
            for (int t = 1; t < rows; t++)
            {
                last += 1/250.0f;
                time.Add(last);
            }

            List<Qualitative> qualis = qualitatives.FindAll(x => x.ID == id && x.Task == (i / 2) - 1);
            if (qualis.Count == 0 || qualis.Count > 1)
            {
                Console.WriteLine($"Falha ao carregar a tarefa {(i/2) - 1} para a coleta {id}: registro qualitativo inválido.");
                continue;
            }

            Qualitative quali = qualis[0];

            Task task = new ()
            {
                Time = time,
                Qualitative = quali.Value,
                YearsOld = quali.YearsOld
            };

            task.Channels[0] = channels[1].ToList();
            task.Channels[1] = channels[2].ToList();
            task.Channels[2] = channels[3].ToList();
            task.Channels[3] = channels[4].ToList();
            task.Channels[4] = channels[5].ToList();
            task.Channels[5] = channels[6].ToList();
            task.Channels[6] = channels[7].ToList();
            task.Channels[7] = channels[8].ToList();

            tasks.Add(task);
        }

        resultList.Add(tasks);
    }

    static void Load (string folder, bool quantitative = true)
    {
        string[] files = Directory.GetFiles(folder);
        foreach (string file in files)
        {
            if (quantitative)
            {
                if (!file.ToLower().EndsWith(".json")) continue;
                quantitativeFiles.Add(file);
            }
            else
            {
                if(!file.ToLower().EndsWith(".csv")) continue;
                qualitativeFiles.Add(file);
            }
        }

        string[] subdirs = Directory.GetDirectories(folder);
        foreach(string dir in subdirs)
            Load(dir, quantitative);
    }

    static void Main (string[] args)
    {
        bool isQualiFile = false, isQualiFolder = false, isQuantiFile = false, isQuantiFolder = false, isResult = false;

        string? resultFile = null;

        foreach (string cmd in args)
        {
            if (cmd == "-q") { isQualiFile = true; isQualiFolder = false; isQualiFile = false; isQuantiFolder = false; isResult = false; continue; }
            if (cmd == "--Fq") { isQualiFile = false; isQualiFolder = true; isQualiFile = false; isQuantiFolder = false; isResult = false; continue; }
            if (cmd == "--d") { isQualiFile = false; isQualiFolder = false; isQualiFile = true; isQuantiFolder = false; isResult = false; continue; }
            if (cmd == "--Fd") { isQualiFile = false; isQualiFolder = false; isQualiFile = false; isQuantiFolder = true; isResult = false; continue; }
            if (cmd == "-o") { isQualiFile = false; isQualiFolder = false; isQualiFile = false; isQuantiFolder = false; isResult = true; continue; }
            

            if (isQualiFile) qualitativeFiles.Add(cmd);
            else if(isQualiFolder) qualitativeFolder.Add(cmd);
            else if(isQuantiFile) quantitativeFiles.Add(cmd);
            else if(isQuantiFolder) quantitativeFolder.Add(cmd);
            else if(isResult) resultFile = cmd;
        }

        if (resultFile == null)
            resultFile = Path.Combine(Directory.GetCurrentDirectory(), "result.json");

        foreach(string dir in qualitativeFolder)
            Load(dir, false);

        foreach(string dir in quantitativeFolder)
            Load(dir);

        foreach (string file in qualitativeFiles)
        {
            StreamReader reader = new (file);
            string[] content = reader.ReadToEnd().Split('\n');
            reader.Close();

            for (int l = 1; l < content.Length; l++)
            {
                string[] columns = content[l].Split(',');
                int id = Convert.ToInt32(columns[1]);
                int yearsOld = Convert.ToInt16(columns[2]);

                for (int c = 3; c < columns.Length; c++)
                {
                    int value;
                    if (columns[c].ToLower().Contains("muito fofo")) value = 10;
                    else if (columns[c].ToLower().Contains("repulsivo")) value = 1;
                    else value = Convert.ToInt32(columns[c]);

                    if (value > 6) value = 1;
                    else if (value < 5) value = -1;
                    else value = 0;

                    qualitatives.Add(new Qualitative() { ID = id, YearsOld = yearsOld, Task = c - 2, Value = value });
                }
            }
        }

        foreach (string file in quantitativeFiles)
        {
            if (!file.ToLower().EndsWith(".json")) continue;
            Parse(file);
        }

        string resultString = JsonSerializer.Serialize(resultList);
        
        if (!File.Exists(resultFile)) File.Create(resultFile).Close();
        StreamWriter writer = new (resultFile);
        writer.Write(resultString);
        writer.Flush();
        writer.Close();
    }
}