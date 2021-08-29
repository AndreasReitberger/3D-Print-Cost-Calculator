using AndreasReitberger;
using AndreasReitberger.Models;
using log4net;
using PrintCostCalculator3d.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace PrintCostCalculator3d.Models.GCode
{
    public static class GcodeChannelWorker
    {
        #region Logger
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public static async Task ProcessGcodesAsync(List<List<string>> filesList, IProgress<int> overallProg)
        {
            List<Gcode> gcodes = new();

            int bufferSize = 100;
            var channel = Channel.CreateBounded<Gcode>(bufferSize);

            var reader = channel.Reader;
            var writer = channel.Writer;

            var listener = Task.Run(() => ListenToGcodeChannel(channel.Reader));

            var cts = new CancellationTokenSource();
            try
            {
                int filecCount = filesList.SelectMany(list => list).Distinct().Count();
                int filesDone = 0;

                foreach (List<string> files in filesList)
                {
                    List<Task> tasks = new();
                    foreach (string file in files)
                    {
                        Gcode gc = new(file);

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                var prog = new Progress<int>(percent =>
                                {
                                    gc.Progress = percent;
                                });
                                gc = await GcodeParser.Instance.FromGcodeAsync(gc, prog, cts.Token, true, null);
                                if (cts.Token.IsCancellationRequested)
                                {
                                    return;
                                }
                                if (gc != null)
                                {

                                    gcodes.Add(gc);
                                    filesDone++;
                                    if (overallProg != null)
                                    {
                                        try
                                        {
                                            float test = (((float)filesDone / filecCount) * 100f);
                                            if (filesDone < filecCount)
                                                overallProg.Report(Convert.ToInt32(test));
                                            else
                                                overallProg.Report(100);
                                        }

                                        catch (Exception exc)
                                        {
                                            logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                        }
                                    }

                                }
                            }
                            catch (Exception exc)
                            {
                                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);

                }
                writer.Complete();

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        static async Task ListenToGcodeChannel(ChannelReader<Gcode> reader)
        {
            List<Gcode> gcodes = new();
            while (await reader.WaitToReadAsync())
            {
                while (reader.TryRead(out Gcode parsedGcoce))
                {
                    gcodes.Add(parsedGcoce);
                }
            }
        }
    }
}
