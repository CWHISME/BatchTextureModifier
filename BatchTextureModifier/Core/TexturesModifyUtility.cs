//=========================================
//作者：wangjiaying@cwhisme
//日期：
//描述：封装 ImageSharp 调用
//用途：https://github.com/CWHISME/BatchTextureModifier.git
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
        /// 支持的格式
        /// </summary>
        public readonly static string[] FormatNames = new string[] { "PNG", "JPG", "Webp", "TGA", "BMP", "GIF" };
        /// <summary>
        /// 支持的文件后缀
        /// </summary>
        public readonly static string[] Filter = new string[] { "*.png", "*.jpg", "*.jpeg", "*.webp", "*.tga", "*.bmp", "*.gif" };

        /// <summary>
        /// 编码器缓存
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
        /// 缩放算法
        /// </summary>
        public readonly static List<IResampler> ResamplerAlgorithms;
        /// <summary>
        /// 缩放算法对应的名字
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
        /// 获取目录下第一张图片
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
        /// 通过下标取格式
        /// </summary>
        /// <returns></returns>
        public static IEncoderSetting? GetFormatByIndex(int index)
        {
            if (index < 0 || index >= Encoder.Length) return null;
            return Encoder[index];
        }

        /// <summary>
        /// 计算大小
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static float CalcSize(byte[] bytes)
        {
            if (bytes == null) return 0;
            return CalcSize(bytes.Length);
        }

        /// <summary>
        /// 计算大小
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
                            //基于宽度，计算新的高度
                            int newHeight = (int)(data.Width / (image.Width / (float)image.Height));
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, (data.Height - newHeight) / 2, data.Width, newHeight), false));
                            break;
                        case EScaleMode.HeightBase:
                            //基于高度，计算新的宽度
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
                            //取最长边作为新的方形变长
                            int pot = CalcPot(image.Width > image.Height ? image.Width : image.Height, data.PotMode);
                            ResizeByMode(image, pot, pot, data, padMode);
                            break;
                    }
                using (MemoryStream ms = new MemoryStream())
                {
                    //检测是原格式还是转换格式
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
                        //判断是否限制最终文件大小
                        IFileSizeSetting? fileSizeSetting = data.OutputEncoder as IFileSizeSetting;
                        IImageEncoder? limitEncoder;
                        int num = 0;
                        do
                        {
                            ms.SetLength(0);
                            limitEncoder = fileSizeSetting?.CreateFileSizeLimitEncoder(num);
                            //已经没法处理了，或者该图格式不支持
                            if (limitEncoder == null)
                            {
                                //直接用默认编码器编码
                                limitEncoder = data.OutputEncoder.CraeteEncoder();
                                image.Save(ms, limitEncoder);
                                if (fileSizeSetting != null) LogManager.GetInstance.Log($"处理图片 {imageName} 过程中，为满足指定文件大小，尝试迭代了 {num} 次后依然失败！已取消限制该文件大小。");
                                num = -1;
                                break;
                            }
                            num++;
                            image.Save(ms, limitEncoder);
                            //文件大小不符合限制，则继续迭代
                        } while (fileSizeSetting != null && fileSizeSetting.MaxFileSize > 1 && CalcSizeKB(ms.Length) > fileSizeSetting.MaxFileSize);
                        if (num > 1) LogManager.GetInstance.Log($"处理图片 {imageName} 过程中，为满足指定文件大小，尝试迭代了 {num} 次后成功。");
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

        //取消token
        private static CancellationTokenSource? _cancellation;
        //当前修改的设置数据
        private static TexturesModifyData? _modifyDataConfig;
        //枚举图片路径
        private static BlockingCollection<string>? _pathQueue;
        //统计数量
        private static int _modifyImageCount;
        //备份目录
        private static string? _backupDirectory;
        //private static string? _outputSuffix;
        //private static Action? _onBatchEnd;

        //public static bool RegisterCancelCallback(Action action)
        //{
        //    if (_cancellation == null)
        //    {
        //        LogManager.GetInstance.LogError("Cancellation 为空，注册出现错误：处理程序未初始化！");
        //        return false;
        //    }
        //    //_cancellation.Token.Register(action);
        //    _onBatchEnd += action;
        //    return true;
        //}

        /// <summary>
        /// 执行批量修改
        /// </summary>
        /// <param name="data"></param>
        public static async Task StartBatchModify(TexturesModifyData data)
        {
            LogManager.GetInstance.LogWarning("\n准备开始执行批量处理......");
            if (data.OutputEncoder == null && data.ScaleMode == EScaleMode.NotScale)
            {
                LogManager.GetInstance.LogError("既没有选择转换格式也没有选择缩放，当前的设置什么都不会改变！");
                return;
            }
            if (_cancellation != null && !_cancellation.IsCancellationRequested)
            {
                LogManager.GetInstance.LogError("当前似乎正在执行中？");
                return;
            }
            if (string.IsNullOrWhiteSpace(data.InputPath))
            {
                LogManager.GetInstance.LogError("无输入目录！");
                return;
            }
            if (string.IsNullOrWhiteSpace(data.OutputPath) && !data.IsDirectOverideFile)
            {
                data.OutputPath = data.InputPath + "_Output";
                if (!TryCreateDirectory(data.OutputPath)) return;
                LogManager.GetInstance.LogWarning("无输出目录！将自动设置为：" + data.OutputPath);
            }
            if (!Directory.Exists(data.OutputPath))
                Directory.CreateDirectory(data.OutputPath!);
            //是否备份，覆盖源文件的情况才需要备份
            if (data.IsDirectOverideFile && data.IsBackupInputFile)
            {
                do
                {
                    _backupDirectory = $"{data.InputPath}_{DateTime.Now.ToString("ss_ffffff")}_Backup";
                } while (Directory.Exists(_backupDirectory));
                LogManager.GetInstance.LogWarning("备份目录自动设置为：" + _backupDirectory);
                if (!TryCreateDirectory(_backupDirectory)) return;
            }
            else _backupDirectory = null;
            _modifyDataConfig = data;
            _cancellation = new CancellationTokenSource();
            Stopwatch stopwatch = Stopwatch.StartNew();
            //初始化路径容器
            _pathQueue = new BlockingCollection<string>(_modifyDataConfig!.ProcessCount);
            _modifyImageCount = 0;
            //并发图片处理线程
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
            LogManager.GetInstance.LogWarning($"批处理执行完毕！共处理 {_modifyImageCount} 个 文件，消耗时间：{Math.Round(stopwatch.ElapsedMilliseconds / 1000f, 2)} 秒");
            //_onBatchEnd?.Invoke();
            //_onBatchEnd = null;
        }

        /// <summary>
        /// 终止批量修改，如有的话
        /// </summary>
        public static void CancelBatch()
        {
            _cancellation?.Cancel();
            LogManager.GetInstance.LogWarning("已请求终止批处理任务...");
        }

        /// <summary>
        /// 枚举目录线程
        /// </summary>
        private static void EnumerateImagesFunc()
        {
            //自身则负责添加路径
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
                            LogManager.GetInstance.LogError("执行超时，已自动停止！");
                            return;
                        }
                        if (_cancellation.Token.IsCancellationRequested)
                        {
                            LogManager.GetInstance.LogWarning("终止枚举线程！");
                            return;
                        }
                    }
                }
                //目录递归结束，该线程也尝试处理一下图片
                while (_pathQueue!.Count > 0 && TryModifyImages()) { }
                LogManager.GetInstance.LogWarning("枚举线程处理完毕！");
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogWarning("枚举线程已强行终止！Error:" + ex.Message);
            }
        }

        /// <summary>
        /// 处理图片线程
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
                    //是否覆盖源文件
                    if (_modifyDataConfig!.IsDirectOverideFile)
                    {
                        //是否备份
                        if (_modifyDataConfig!.IsBackupInputFile && !string.IsNullOrEmpty(_backupDirectory))
                        {
                            //执行备份
                            string backupPath = _modifyDataConfig!.IsBackupWithStructure ? Path.Combine(_backupDirectory, path.Replace(_modifyDataConfig!.InputPath!, "")) : Path.Combine(_backupDirectory, Path.GetFileName(path));
                            if (TryCreateDirectory(Path.GetDirectoryName(backupPath)!))
                                File.WriteAllBytes(backupPath, bytes);
                        }
                        //删除源文件
                        File.Delete(path);
                    }
                    //执行修改
                    byte[] bytesOut = ModifyTextures(bytes, _modifyDataConfig!, Path.GetFileName(path));
                    //处理后新文件
                    string newPath = Path.Combine(_modifyDataConfig!.OutputPath!, Path.GetFileNameWithoutExtension(path) + _modifyDataConfig.OutputEncoder?.GetFileSuffix() ?? Path.GetExtension(path));
                    File.WriteAllBytes(newPath, bytesOut);
                    Interlocked.Add(ref _modifyImageCount, 1);
                    LogManager.GetInstance.Log($"线程 [{markId}] 处理完成：\n {path} ({CalcSize(bytes)}M) ---> \n {newPath} ({CalcSize(bytesOut)}M)");
                }
                if (_cancellation.Token.IsCancellationRequested)
                {
                    LogManager.GetInstance.Log("终止图片处理线程：" + markId);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogWarning($"图片处理线程 [{markId}] 已强行终止！Error:{ex.Message}");
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
                LogManager.GetInstance.LogError("创建备份目录失败：" + ex.Message);
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
            //先计算原图比例
            float scaleRatio = image.Width / (float)image.Height;
            //再按照比例映射至新图时，什么分辨率合适
            //保持不变的情况下，若基于宽度
            int newHeight = (int)(data.Width / scaleRatio);
            //保持不变的情况下，若基于高度
            int newWidth = (int)(data.Height * scaleRatio);
            //新的正确比例情况下，哪个正确不越界
            if (newHeight > data.Height)
            {
                //基于宽度，新的高度越界了那么，取基于高度
                newHeight = data.Height;
            }
            else
            {
                //基于高度，新的宽度越界了那么，取基于宽度
                newWidth = data.Width;
            }
            startX = (data.Width - newWidth) / 2;
            startY = (data.Height - newHeight) / 2;

            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(startX, startY, newWidth, newHeight), false));
        }

        /// <summary>
        /// 计算给定数值的 POT（根据缩放模式返回给定数值接近的2N数值）
        /// 最大支持 65535
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
                    //判断哪个更接近 X
                    return x - oldX > oldX - minPot ? minPot : x;
                case EPotMode.ToLarger:
                    //比X大的 2N 数值
                    return x;
                case EPotMode.ToSmaller:
                    //比X小的 2N 数值
                    return minPot;
            }
            //出问题了？
            return oldX;
        }
    }
}