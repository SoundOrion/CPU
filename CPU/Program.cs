using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;

// Windows 環境限定
// NumberOfCores → 物理コア数
// NumberOfLogicalProcessors → 論理スレッド数（HT含む）
using (var searcher = new ManagementObjectSearcher("select * from Win32_Processor"))
{
    foreach (var item in searcher.Get())
    {
        Console.WriteLine($"CPU 名: {item["Name"]}");
        Console.WriteLine($"コア数: {item["NumberOfCores"]}");
        Console.WriteLine($"論理プロセッサ数: {item["NumberOfLogicalProcessors"]}");
        Console.WriteLine($"クロック速度 (MHz): {item["MaxClockSpeed"]}");
        Console.WriteLine($"メーカー: {item["Manufacturer"]}");
        Console.WriteLine($"プロセッサID: {item["ProcessorId"]}");
        Console.WriteLine($"アーキテクチャ: {item["Architecture"]}");
        Console.WriteLine($"説明: {item["Description"]}");
    }
}

Console.WriteLine("");

// クロスプラットフォーム対応
// RuntimeInformation.ProcessArchitecture → CPU アーキテクチャ（x86, x64, ARMなど）
// RuntimeInformation.OSArchitecture → OSのアーキテクチャ（64bit, 32bit など）
Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
Console.WriteLine($"アーキテクチャ (プロセッサ): {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"アーキテクチャ (OS): {RuntimeInformation.OSArchitecture}");
Console.WriteLine($"論理プロセッサ数: {Environment.ProcessorCount}");

Console.WriteLine("");

// Process.GetCurrentProcess().Threads.Count → 実行中のプロセスのスレッド数を取得
Console.WriteLine($"論理プロセッサ数: {Environment.ProcessorCount}");

using (Process process = Process.GetCurrentProcess())
{
    Console.WriteLine($"現在のプロセスのスレッド数: {process.Threads.Count}");
}

// スレッドを増やして変化を見る
for (int i = 0; i < 10; i++)
{
    new Thread(() =>
    {
        Thread.Sleep(5000);
    }).Start();
}

using (Process process = Process.GetCurrentProcess())
{
    Console.WriteLine($"スレッド増加後のスレッド数: {process.Threads.Count}");
}



// 「最適なスレッド数」はどう決める？
await Parallel.ForEachAsync(Enumerable.Range(0, 1000),
    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
    async (i, token) =>
    {
        Console.WriteLine($"スレッド {Thread.CurrentThread.ManagedThreadId} で {i} を処理");
        await Task.Delay(1000);
    });

//private static readonly SemaphoreSlim _semaphore = new(Environment.ProcessorCount, Environment.ProcessorCount);

//public async Task RunTasksWithLimitAsync()
//{
//    var tasks = Enumerable.Range(0, 100).Select(async i =>
//    {
//        await _semaphore.WaitAsync();
//        try
//        {
//            await Task.Delay(1000); // 実際の処理に置き換え
//            Console.WriteLine($"スレッド {Thread.CurrentThread.ManagedThreadId} で {i} を処理");
//        }
//        finally
//        {
//            _semaphore.Release();
//        }
//    }).ToArray();

//    await Task.WhenAll(tasks);
//}

