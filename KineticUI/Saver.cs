using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.JSConverters;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Sim;

namespace KineticUI
{
    public class Saver
    {
        private static readonly object _lock = new object();
        private static bool _saving = false;
        private static bool _saveRequested = false;
        private volatile static int _saveFile = 1;
        

        public static void LocalSave()
        {
            bool doSave;
            lock (_lock)
            {
                doSave = !_saving;
                _saving = true;
                if (!doSave)
                {
                    _saveRequested = true;
                }
            }
            if (doSave)
            {
                Parallel.Invoke(DoSave);
            }
        }

        private static string GetFileName(int no)
        {
            return "SaveFile" + no + ".json";
        }

        private static void DoSave()
        {
            string fileName = GetFileName(_saveFile);
            _saveFile = (_saveFile + 1)%2;
            System.Diagnostics.Debug.WriteLine("Saving state to: " + fileName);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (StreamWriter stream = File.AppendText(fileName))
            {
                Save(stream);
            }

            //set state to not be saving and re-run if another save has been requested
            bool saveAgain = false;
            lock (_lock)
            {
                if (_saveRequested)
                {
                    saveAgain = true;
                }
                _saving = false;
                _saveRequested = false;
            }

            if (saveAgain)
            {
                LocalSave();
            }
        }

        public static void Save(StreamWriter writer)
        {
            Serializer.Ser.Formatting = Formatting.Indented;

            writer.WriteLine("###Fixtures\n");
            foreach (var kv in LightSystemProvider.getFixtures())
            {
                writer.Write(kv.Key);
                writer.Write(" ");
                writer.Write(kv.Value);
                writer.WriteLine();
            }
            writer.WriteLine("\n###Lights\n");
            foreach (var node in LightSystemProvider.Lights)
            {
                writer.Write(node.Address.ToString());
                writer.Write(' ');
                writer.Write(node.Position.X);
                writer.Write(' ');
                writer.Write(node.Position.Y);
                writer.Write(' ');
                writer.Write(node.Position.Z);
                writer.WriteLine();
            }

            writer.WriteLine("\n\n### Groups\n");
            writer.Flush();
            Serializer.ToStream(State.Scene.Groups, writer.BaseStream);
            //            writer.Write(groups);
            //            groups = null;


            writer.WriteLine("\n\n### Patterns\n");
            writer.Flush();
            Serializer.ToStream(State.Scene.Patterns, writer.BaseStream);
            //            writer.WriteLine(patterns);
            //            patterns = null;
            writer.Flush();

            writer.WriteLine("\n\n### Composition");
            foreach (var start in State.Simulation.PatternProvider.PatternStarts)
            {
                writer.WriteLine(string.Join(",", new[]
                                                      {
                                                          start.Pattern.Name,
                                                          start.StartTime.ToString(),
                                                          start.Id.ToString(),
                                                          start.Priority.ToString()
                                                      }));
            }

            Serializer.Ser.Formatting = Formatting.None;
        }

        private static void GetFiles(out string primary, out string fallback)
        {
            primary = fallback = null;
            if (!File.Exists(GetFileName(0)) && !File.Exists(GetFileName(1)))
            {
                System.Diagnostics.Debug.WriteLine("No save file to load from");
            }
            else if (!File.Exists(GetFileName(0)))
            {
                primary = GetFileName(1);
            }
            else if (!File.Exists(GetFileName(1)))
            {
                primary = GetFileName(0);
            }
            else if (File.GetLastWriteTime(GetFileName(0)) > File.GetLastWriteTime(GetFileName(1)))
            {
                primary = GetFileName(0);
                fallback =GetFileName(1);
            }
            else
            {
                primary = GetFileName(1);
                fallback = GetFileName(0);
            }
        }

        public static void LocalLoad()
        {
            string primary, fallback;
            GetFiles(out primary, out fallback);
            if (primary == null)
            {
                return;
            }


            Stream s1=null, s2=null;
            System.Diagnostics.Debug.WriteLine("Loading save file...");
            try
            {
                s1 = File.OpenRead(primary);
                Load(s1);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error while attempting to load most recent save file: " + e.Message);
                if (s1 != null)
                {
                    s1.Close();
                    s1 = null;
                }
                if (fallback != null)
                {
                    System.Diagnostics.Debug.WriteLine("Attempting to load previous save file...");
                    try
                    {
                        s2 = File.OpenRead(fallback);
                        Load(s2);
                    }
                    catch (Exception e2)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to load fallback save file: " + e2.Message);
                    }
                }
            }
            finally
            {
                if (s1 != null)
                {
                    s1.Close();
                }
                if (s2 != null)
                {
                    s2.Close();
                }
            }
            System.Diagnostics.Debug.WriteLine("Successfully loaded save state");
        }

        public static void Load(Stream stream)
        {
            IDictionary<string, string> nameToSection;
            LightSystemProvider.ParseProps(stream, out nameToSection);
            State.Scene = new Scene();
            State.PatternSim = ReadAheadSimulation.TempSimulation(State.Scene);
            State.Simulation = ReadAheadSimulation.TempSimulation(State.Scene);

            if (nameToSection.ContainsKey("Groups"))
            {
                string section = nameToSection["Groups"];
                foreach (var group in Serializer.FromString<IEnumerable<IGroup>>(section))
                {
                    State.Scene.SetGroup(group);
                }
            }

            if (nameToSection.ContainsKey("Patterns"))
            {
                string section = nameToSection["Patterns"];
                foreach (var pattern in Serializer.FromString<IEnumerable<Pattern>>(section))
                {
                    State.Scene.SetPattern(pattern);
                }
            }

            if (nameToSection.ContainsKey("Composition"))
            {
                var split = nameToSection["Composition"].Split(new[]{"\r\n","\n"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in split)
                {
                    var props = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int startTime, id, priority;
                    if (props.Count() != 4 || !int.TryParse(props[1], out startTime) 
                          || !int.TryParse(props[2], out id) || !int.TryParse(props[3], out priority))
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid Composition Line: " + line);
                        continue;
                    }
                    State.Simulation.PatternProvider.AddPattern(props[0], startTime, id, priority);
                }
            }
            stream.Close();
        }
    }
}