//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�
//��������װ ImageSharp ����
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace BatchTextureModifier
{
    public static class TexturesModifyUtility
    {
        /// <summary>
        /// ֧�ֵĸ�ʽ
        /// </summary>
        public readonly static string[] FormatNames = new string[] { "PNG", "JPG", "Webp", "TGA", "BMP", "GIF" };
        /// <summary>
        /// ֧�ֵ��ļ���׺
        /// </summary>
        public readonly static string[] Filter = new string[] { "*.png", "*.jpg", "*.jpeg", "*.webp", "*.tga", "*.bmp", "*.gif" };

        /// <summary>
        /// ����������
        /// </summary>
        public readonly static IEncoderSetting[] Encoder = new IEncoderSetting[] {
            new PngEncoderSetting(),
            new JpegEncoderSetting(),
            new WebpEncoderSetting(),
            new TgaEncoderSetting(),
            new BmpEncoderSetting(),
            new GifEncoderSetting()
        };

        /// <summary>
        /// �����㷨
        /// </summary>
        public readonly static List<IResampler> ResamplerAlgorithms;
        /// <summary>
        /// �����㷨��Ӧ������
        /// </summary>
        public readonly static List<string> ResamplerAlgorithmNames;

        static TexturesModifyUtility()
        {
            PropertyInfo[] prt = typeof(SixLabors.ImageSharp.Processing.KnownResamplers).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            ResamplerAlgorithms = new List<IResampler>(prt.Length);
            ResamplerAlgorithmNames = new List<string>(prt.Length);
            foreach (var item in prt)
            {
                IResampler? resampler = item.GetValue(null) as IResampler;
                if (resampler == null) continue;
                ResamplerAlgorithms.Add(resampler);
                ResamplerAlgorithmNames.Add(item.Name);
            }
        }

        /// <summary>
        /// ��ȡĿ¼�µ�һ��ͼƬ
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string GetDirectoryFirstTextures(string dirPath)
        {
            try
            {
                EnumerationOptions opt = new EnumerationOptions();
                opt.RecurseSubdirectories = true;
                foreach (var pattern in Filter)
                {
                    foreach (var item in Directory.EnumerateFiles(dirPath, pattern, opt))
                    {
                        return item;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.GetInstance.LogError(ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// ͨ���±�ȡ��ʽ
        /// </summary>
        /// <returns></returns>
        public static IEncoderSetting? GetFormatByIndex(int index)
        {
            if (index < 0 || index >= Encoder.Length) return null;
            return Encoder[index];
        }

        /// <summary>
        /// �����С
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static float CalcSize(byte[] bytes)
        {
            if (bytes == null) return 0;
            return CalcSize(bytes.Length);
        }

        /// <summary>
        /// �����С
        /// </summary>
        /// <returns></returns>
        public static float CalcSize(long length)
        {
            float m = 1024 * 1024;
            return (float)(Math.Round(length / m, length < m ? 3 : 2));
        }

        public static int CalcSizeKB(long length)
        {
            return (int)Math.Floor(length / 1024f);
        }

        public static byte[] ModifyTextures(byte[] bytes, TexturesModifyData data, string? imageName = null)
        {
            //if (data.ScaleMode == EScaleMode.NotScale && data.OutputEncoder == null) return bytes;
            //try
            //{
            //Image<Rgba32> 
            SixLabors.ImageSharp.Processing.ResizeMode padMode = data.StayPixel ? SixLabors.ImageSharp.Processing.ResizeMode.BoxPad : SixLabors.ImageSharp.Processing.ResizeMode.Pad;
            using (Image image = Image.Load(bytes))
            {
                if (data.ScaleMode != EScaleMode.NotScale)
                    switch (data.ScaleMode)
                    {
                        case EScaleMode.ScaleBased:
                            ResizeByScaleBased(data, image);
                            break;
                        case EScaleMode.DirectCut:
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), false));
                            break;
                        case EScaleMode.WidthBase:
                            //���ڿ�ȣ������µĸ߶�
                            int newHeight = (int)(data.Width / (image.Width / (float)image.Height));
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, (data.Height - newHeight) / 2, data.Width, newHeight), false));
                            break;
                        case EScaleMode.HeightBase:
                            //���ڸ߶ȣ������µĿ��
                            int newWidth = (int)(data.Height * (image.Width / (float)image.Height));
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle((data.Width - newWidth) / 2, 0, newWidth, data.Height), false));
                            break;
                        case EScaleMode.ScaleBasedByCut:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Crop);
                            break;
                        case EScaleMode.Pad:
                            ResizeByMode(image, data, padMode);
                            break;
                        case EScaleMode.DirectScale_Min:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Min);
                            break;
                        case EScaleMode.DirectScale_Max:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Max);
                            break;
                        case EScaleMode.StretchScale:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Stretch);
                            break;
                        case EScaleMode.POT:
                            ResizeByMode(image, CalcPot(image.Width, data.PotMode), CalcPot(image.Height, data.PotMode), data, padMode);
                            break;
                        case EScaleMode.POT_Cube:
                            //ȡ�����Ϊ�µķ��α䳤
                            int pot = CalcPot(image.Width > image.Height ? image.Width : image.Height, data.PotMode);
                            ResizeByMode(image, pot, pot, data, padMode);
                            break;
                    }
                using (MemoryStream ms = new MemoryStream())
                {
                    //�����ԭ��ʽ����ת����ʽ
                    if (data.OutputEncoder == null)
                    {
                        IImageFormat? format = Image.DetectFormat(bytes);
                        //if (Image.TryDetectFormat(bytes, out format))
                        if (format != null)
                            image.Save(ms, format);
                        else image.Save(ms, GetDefaultEncoder());
                    }
                    else
                    {
                        //�ж��Ƿ����������ļ���С
                        IFileSizeSetting? fileSizeSetting = data.OutputEncoder as IFileSizeSetting;
                        IImageEncoder? limitEncoder;
                        int num = 0;
                        do
                        {
                            ms.SetLength(0);
                            limitEncoder = fileSizeSetting?.CreateFileSizeLimitEncoder(num);
                            //�Ѿ�û�������ˣ����߸�ͼ��ʽ��֧��
                            if (limitEncoder == null)
                            {
                                //ֱ����Ĭ�ϱ���������
                                limitEncoder = data.OutputEncoder.CraeteEncoder();
                                image.Save(ms, limitEncoder);
                                if (fileSizeSetting != null) LogManager.GetInstance.Log($"����ͼƬ {imageName} �����У�Ϊ����ָ���ļ���С�����Ե����� {num} �κ���Ȼʧ�ܣ���ȡ�����Ƹ��ļ���С��");
                                num = -1;
                                break;
                            }
                            num++;
                            image.Save(ms, limitEncoder);
                            //�ļ���С���������ƣ����������
                        } while (fileSizeSetting != null && fileSizeSetting.MaxFileSize > 1 && CalcSizeKB(ms.Length) > fileSizeSetting.MaxFileSize);
                        if (num > 1) LogManager.GetInstance.Log($"����ͼƬ {imageName} �����У�Ϊ����ָ���ļ���С�����Ե����� {num} �κ�ɹ���");
                    }
                    return ms.ToArray();
                }
                //image.Save(Path.Combine(Path.GetDirectoryName(path), "[Resize]" + Path.GetFileName(path)));
            }
            //}
            //catch (Exception ex)
            //{
            //    LogManager.GetInstance.LogError(ex.Message);
            //    return null;
            //}
        }

        //ȡ��token
        private static CancellationTokenSource? _cancellation;
        //��ǰ�޸ĵ���������
        private static TexturesModifyData? _modifyDataConfig;
        //ö��ͼƬ·��
        private static BlockingCollection<string>? _pathQueue;
        //ͳ������
        private static int _modifyImageCount;
        //����Ŀ¼
        private static string? _backupDirectory;
        //private static string? _outputSuffix;
        //private static Action? _onBatchEnd;

        //public static bool RegisterCancelCallback(Action action)
        //{
        //    if (_cancellation == null)
        //    {
        //        LogManager.GetInstance.LogError("Cancellation Ϊ�գ�ע����ִ��󣺴������δ��ʼ����");
        //        return false;
        //    }
        //    //_cancellation.Token.Register(action);
        //    _onBatchEnd += action;
        //    return true;
        //}

        /// <summary>
        /// ִ�������޸�
        /// </summary>
        /// <param name="data"></param>
        public static async Task StartBatchModify(TexturesModifyData data)
        {
            LogManager.GetInstance.LogWarning("\n׼����ʼִ����������......");
            if (data.OutputEncoder == null && data.ScaleMode == EScaleMode.NotScale)
            {
                LogManager.GetInstance.LogError("��û��ѡ��ת����ʽҲû��ѡ�����ţ���ǰ������ʲô������ı䣡");
                return;
            }
            if (_cancellation != null && !_cancellation.IsCancellationRequested)
            {
                LogManager.GetInstance.LogError("��ǰ�ƺ�����ִ���У�");
                return;
            }
            if (string.IsNullOrWhiteSpace(data.InputPath))
            {
                LogManager.GetInstance.LogError("������Ŀ¼��");
                return;
            }
            if (string.IsNullOrWhiteSpace(data.OutputPath) && !data.IsDirectOverideFile)
            {
                data.OutputPath = data.InputPath + "_Output";
                if (!TryCreateDirectory(data.OutputPath)) return;
                LogManager.GetInstance.LogWarning("�����Ŀ¼�����Զ�����Ϊ��" + data.OutputPath);
            }
            if (!Directory.Exists(data.OutputPath))
                Directory.CreateDirectory(data.OutputPath!);
            //�Ƿ񱸷ݣ�����Դ�ļ����������Ҫ����
            if (data.IsDirectOverideFile && data.IsBackupInputFile)
            {
                do
                {
                    _backupDirectory = $"{data.InputPath}_{DateTime.Now.ToString("ss_ffffff")}_Backup";
                } while (Directory.Exists(_backupDirectory));
                LogManager.GetInstance.LogWarning("����Ŀ¼�Զ�����Ϊ��" + _backupDirectory);
                if (!TryCreateDirectory(_backupDirectory)) return;
            }
            else _backupDirectory = null;
            _modifyDataConfig = data;
            _cancellation = new CancellationTokenSource();
            Stopwatch stopwatch = Stopwatch.StartNew();
            //��ʼ��·������
            _pathQueue = new BlockingCollection<string>(_modifyDataConfig!.ProcessCount);
            _modifyImageCount = 0;
            //����ͼƬ�����߳�
            Task[] tasks = new Task[_modifyDataConfig.ProcessCount];
            for (int i = 0; i < _modifyDataConfig.ProcessCount; i++)
            {
                int index = i + 1;
                tasks[i] = Task.Run(() => ModifyImagesFunc(index));
            }
            await Task.Run(EnumerateImagesFunc);
            _cancellation.Cancel();
            await Task.WhenAll(tasks);
            _cancellation.Dispose();
            _pathQueue.Dispose();
            stopwatch.Stop();
            LogManager.GetInstance.LogWarning($"������ִ����ϣ������� {_modifyImageCount} �� �ļ�������ʱ�䣺{Math.Round(stopwatch.ElapsedMilliseconds / 1000f, 2)} ��");
            //_onBatchEnd?.Invoke();
            //_onBatchEnd = null;
        }

        /// <summary>
        /// ��ֹ�����޸ģ����еĻ�
        /// </summary>
        public static void CancelBatch()
        {
            _cancellation?.Cancel();
            LogManager.GetInstance.LogWarning("��������ֹ����������...");
        }

        /// <summary>
        /// ö��Ŀ¼�߳�
        /// </summary>
        private static void EnumerateImagesFunc()
        {
            //�����������·��
            EnumerationOptions opt = new EnumerationOptions();
            opt.RecurseSubdirectories = true;
            try
            {
                foreach (var pattern in Filter)
                {
                    foreach (var item in Directory.EnumerateFiles(_modifyDataConfig!.InputPath!, pattern, opt))
                    {
                        if (!_pathQueue!.TryAdd(item, _modifyDataConfig.ProcessWaitMillisecond, _cancellation!.Token))
                        {
                            LogManager.GetInstance.LogError("ִ�г�ʱ�����Զ�ֹͣ��");
                            return;
                        }
                        if (_cancellation.Token.IsCancellationRequested)
                        {
                            LogManager.GetInstance.LogWarning("��ֹö���̣߳�");
                            return;
                        }
                    }
                }
                //Ŀ¼�ݹ���������߳�Ҳ���Դ���һ��ͼƬ
                while (_pathQueue!.Count > 0 && TryModifyImages()) { }
                LogManager.GetInstance.LogWarning("ö���̴߳�����ϣ�");
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogWarning("ö���߳���ǿ����ֹ��Error:" + ex.Message);
            }
        }

        /// <summary>
        /// ����ͼƬ�߳�
        /// </summary>
        private static void ModifyImagesFunc(int num)
        {
            while (TryModifyImages(num)) { }
        }

        private static bool TryModifyImages(int markId = -1)
        {
            Thread.Sleep(100);
            try
            {
                string? path;
                if (_pathQueue!.TryTake(out path, 1, _cancellation!.Token))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    //�Ƿ񸲸�Դ�ļ�
                    if (_modifyDataConfig!.IsDirectOverideFile)
                    {
                        //�Ƿ񱸷�
                        if (_modifyDataConfig!.IsBackupInputFile && !string.IsNullOrEmpty(_backupDirectory))
                        {
                            //ִ�б���
                            string backupPath = _modifyDataConfig!.IsBackupWithStructure ? Path.Combine(_backupDirectory, path.Replace(_modifyDataConfig!.InputPath!, "")) : Path.Combine(_backupDirectory, Path.GetFileName(path));
                            if (TryCreateDirectory(Path.GetDirectoryName(backupPath)!))
                                File.WriteAllBytes(backupPath, bytes);
                        }
                        //ɾ��Դ�ļ�
                        File.Delete(path);
                    }
                    //ִ���޸�
                    byte[] bytesOut = ModifyTextures(bytes, _modifyDataConfig!, Path.GetFileName(path));
                    //��������ļ�
                    string newPath = Path.Combine(_modifyDataConfig!.OutputPath!, Path.GetFileNameWithoutExtension(path) + _modifyDataConfig.OutputEncoder?.GetFileSuffix() ?? Path.GetExtension(path));
                    File.WriteAllBytes(newPath, bytesOut);
                    Interlocked.Add(ref _modifyImageCount, 1);
                    LogManager.GetInstance.Log($"�߳� [{markId}] ������ɣ�\n {path} ({CalcSize(bytes)}M) ---> \n {newPath} ({CalcSize(bytesOut)}M)");
                }
                if (_cancellation.Token.IsCancellationRequested)
                {
                    LogManager.GetInstance.Log("��ֹͼƬ�����̣߳�" + markId);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogWarning($"ͼƬ�����߳� [{markId}] ��ǿ����ֹ��Error:{ex.Message}");
                return false;
            }
        }

        private static bool TryCreateDirectory(string dir)
        {
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir); return true;
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogError("��������Ŀ¼ʧ�ܣ�" + ex.Message);
                return false;
            }
        }

        private static IImageEncoder GetDefaultEncoder()
        {
            return Encoder[0].CraeteEncoder();
        }

        private static void ResizeByMode(Image image, TexturesModifyData data, ResizeMode mode)
        {
            ResizeByMode(image, data.Width, data.Height, data, mode);
        }

        private static void ResizeByMode(Image image, int width, int height, TexturesModifyData data, ResizeMode mode)
        {
            ResizeOptions opt = new ResizeOptions();
            opt.Position = data.ImageScaleAnchorPositionMode;
            opt.Sampler = data.ResamplerAlgorithm;
            opt.Size = new SixLabors.ImageSharp.Size(width, height);
            opt.Mode = mode;
            image.Mutate(x => x.Resize(opt));
        }

        private static void ResizeByScaleBased(TexturesModifyData data, Image image)
        {
            int startX = 0, startY = 0;
            //�ȼ���ԭͼ����
            float scaleRatio = image.Width / (float)image.Height;
            //�ٰ��ձ���ӳ������ͼʱ��ʲô�ֱ��ʺ���
            //���ֲ��������£������ڿ��
            int newHeight = (int)(data.Width / scaleRatio);
            //���ֲ��������£������ڸ߶�
            int newWidth = (int)(data.Height * scaleRatio);
            //�µ���ȷ��������£��ĸ���ȷ��Խ��
            if (newHeight > data.Height)
            {
                //���ڿ�ȣ��µĸ߶�Խ������ô��ȡ���ڸ߶�
                newHeight = data.Height;
            }
            else
            {
                //���ڸ߶ȣ��µĿ��Խ������ô��ȡ���ڿ��
                newWidth = data.Width;
            }
            startX = (data.Width - newWidth) / 2;
            startY = (data.Height - newHeight) / 2;

            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(startX, startY, newWidth, newHeight), false));
        }

        /// <summary>
        /// ���������ֵ�� POT����������ģʽ���ظ�����ֵ�ӽ���2N��ֵ��
        /// ���֧�� 65535
        /// </summary>
        /// <returns></returns>
        private static int CalcPot(int x, EPotMode mode)
        {
            int oldX = x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x += 1;
            int minPot = x >> 1;
            switch (mode)
            {
                case EPotMode.ToNearest:
                    //�ж��ĸ����ӽ� X
                    return x - oldX > oldX - minPot ? minPot : x;
                case EPotMode.ToLarger:
                    //��X��� 2N ��ֵ
                    return x;
                case EPotMode.ToSmaller:
                    //��XС�� 2N ��ֵ
                    return minPot;
            }
            //�������ˣ�
            return oldX;
        }
    }
}