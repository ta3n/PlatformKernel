using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace Liberty.ServiceDefaults;

/// <summary>
/// Provides functionality to log runtime information or retrieve it as text.
/// This class is intended to be used for diagnostic or informational purposes,
/// allowing services to expose runtime details or log them during initialization.
/// </summary>
public static class LogRuntimeInfo
{
    /// <summary>
    /// Represents the fixed indentation string used in runtime logging output.
    /// </summary>
    /// <remarks>
    /// This constant is typically used to prefix log messages to visually organize
    /// and structure nested information within the log output.
    /// </remarks>
    private const string Indent = "  • ";

    /// <summary>
    /// Represents the default value indicating that a specific setting has not been explicitly configured.
    /// </summary>
    /// <remarks>
    /// This constant is commonly used in scenarios where a required configuration value
    /// or property is missing and must be clearly identified as unset or uninitialized.
    /// </remarks>
    private const string ValueNotSet = "not set";

    /// <summary>
    /// Logs runtime information for the application or service.
    /// </summary>
    /// <remarks>
    /// This method captures and records runtime details to aid in diagnostics, monitoring,
    /// or logging system behavior across different environments.
    /// Ensure proper configuration of logging mechanisms to effectively collect the output.
    /// </remarks>
    public static void Log()
    {
        Console.WriteLine("========== .NET RUNTIME CONFIGURATION ==========");

        Console.WriteLine("Runtime Information:");
        Console.WriteLine($"{Indent}OS Version: {Environment.OSVersion}");
        Console.WriteLine($"{Indent}Framework Description: {Environment.Version}");
        Console.WriteLine($"{Indent}Machine Name: {Environment.MachineName}");
        Console.WriteLine($"{Indent}User Domain Name: {Environment.UserDomainName}");
        Console.WriteLine($"{Indent}User Name: {Environment.UserName}");
        Console.WriteLine($"{Indent}Processor Count: {Environment.ProcessorCount}");
        Console.WriteLine($"{Indent}Current Directory: {Environment.CurrentDirectory}");

        // Basic information
        Console.WriteLine($"{Indent}CLR Version: {Environment.Version}");
        Console.WriteLine($"{Indent}Framework Description: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"{Indent}Process Architecture: {RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"{Indent}OS Description: {RuntimeInformation.OSDescription}");
        Console.WriteLine($"{Indent}OS Architecture: {RuntimeInformation.OSArchitecture}");

        // Runtime environment information
        Console.WriteLine("Environment Variables:");
        Console.WriteLine($"{Indent}DOTNET_GCHeapCount: {Environment.GetEnvironmentVariable("DOTNET_GCHeapCount") ?? ValueNotSet}");
        Console.WriteLine($"{Indent}DOTNET_GCCpuGroup: {Environment.GetEnvironmentVariable("DOTNET_GCCpuGroup") ?? ValueNotSet}");
        Console.WriteLine($"{Indent}DOTNET_ReadyToRun: {Environment.GetEnvironmentVariable("DOTNET_ReadyToRun") ?? ValueNotSet}");
        Console.WriteLine(
            $"{Indent}DOTNET_TieredCompilation: {Environment.GetEnvironmentVariable("DOTNET_TieredCompilation") ?? ValueNotSet}"
        );
        Console.WriteLine(
            $"{Indent}DOTNET_TC_QuickJitForLoops: {Environment.GetEnvironmentVariable("DOTNET_TC_QuickJitForLoops") ?? ValueNotSet}"
        );
        Console.WriteLine($"{Indent}Is Globalization Invariant: {IsGlobalizationInvariant()}");

        // GC information
        Console.WriteLine("GC Configuration:");
        Console.WriteLine($"{Indent}IsServerGC: {GCSettings.IsServerGC}");
        Console.WriteLine($"{Indent}GC Mode: {(GCSettings.IsServerGC ? "Server" : "Workstation")}");
        Console.WriteLine($"{Indent}LatencyMode: {GCSettings.LatencyMode}");
        Console.WriteLine($"{Indent}LargeObjectHeapCompactionMode: {GCSettings.LargeObjectHeapCompactionMode}");

        // RetainVM status
        var (isEnabled, source) = GetRetainVmStatus();
        Console.WriteLine($"{Indent}RetainVM: {isEnabled}");
        Console.WriteLine($"{Indent}RetainVM Source: {source}");

        // Detailed information about the number of GC heaps and cores
        var (estimatedHeapCount, threadsPerHeap) = GetDetailedGcInfo();
        Console.WriteLine($"{Indent}Estimated GC Heaps: {estimatedHeapCount}");
        Console.WriteLine($"{Indent}Total Processors: {Environment.ProcessorCount}");
        Console.WriteLine($"{Indent}GC Threads Per Heap: {threadsPerHeap}");

        try
        {
            Console.WriteLine($"{Indent}LOH Size: {GC.GetGCMemoryInfo().HeapSizeBytes / (1024 * 1024)} MB");
            Console.WriteLine($"{Indent}Total Available Memory: {GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024)} MB");

            if (Environment.Version.Major >= 5)
            {
                Console.WriteLine(
                    $"{Indent}High Memory Load Threshold: {GC.GetGCMemoryInfo().HighMemoryLoadThresholdBytes / (1024 * 1024)} MB"
                );
                Console.WriteLine($"{Indent}Total Committed Bytes: {GC.GetGCMemoryInfo().TotalCommittedBytes / (1024 * 1024)} MB");
                Console.WriteLine($"{Indent}Promoted Bytes: {GC.GetGCMemoryInfo().PromotedBytes / (1024 * 1024)} MB");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not get GC Memory Info: {ex.Message}");
        }

        // Globalization configuration information
        Console.WriteLine("Globalization Configuration:");
        Console.WriteLine($"{Indent}CurrentCulture: {CultureInfo.CurrentCulture.Name}");
        Console.WriteLine($"{Indent}CurrentUICulture: {CultureInfo.CurrentUICulture.Name}");
        Console.WriteLine($"{Indent}InvariantCulture: {CultureInfo.InvariantCulture.Name}");

        // Time resolution and performance information
        Console.WriteLine("Performance Configuration:");
        Console.WriteLine($"{Indent}Stopwatch.Frequency: {Stopwatch.Frequency} ticks/second");
        Console.WriteLine($"{Indent}Stopwatch.IsHighResolution: {Stopwatch.IsHighResolution}");
        Console.WriteLine($"{Indent}DateTime.UtcNow Precision: {GetDateTimePrecision()} ms");

        // ThreadPool configuration information
        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
        var availableThreads = GetAvailableThreads();

        Console.WriteLine("ThreadPool Configuration:");
        Console.WriteLine($"{Indent}Min Worker Threads: {minWorkerThreads}");
        Console.WriteLine($"{Indent}Min Completion Port Threads: {minCompletionPortThreads}");
        Console.WriteLine($"{Indent}Max Worker Threads: {maxWorkerThreads}");
        Console.WriteLine($"{Indent}Max Completion Port Threads: {maxCompletionPortThreads}");
        Console.WriteLine($"{Indent}Available Worker Threads: {availableThreads.Item1}");
        Console.WriteLine($"{Indent}Available Completion Port Threads: {availableThreads.Item2}");

        // JIT information
        Console.WriteLine("JIT Configuration:");
        Console.WriteLine($"{Indent}Is Tiered Compilation Enabled: {IsTieredCompilationEnabled()}");
        Console.WriteLine($"{Indent}Is ReadyToRun Enabled: {IsReadyToRunEnabled()}");

        // Memory information
        var process = Process.GetCurrentProcess();
        Console.WriteLine("Process Memory:");
        Console.WriteLine($"{Indent}Working Set: {process.WorkingSet64 / (1024 * 1024)} MB");
        Console.WriteLine($"{Indent}Private Memory: {process.PrivateMemorySize64 / (1024 * 1024)} MB");
        Console.WriteLine($"{Indent}Virtual Memory: {process.VirtualMemorySize64 / (1024 * 1024)} MB");
        Console.WriteLine($"{Indent}Paged Memory: {process.PagedMemorySize64 / (1024 * 1024)} MB");
        Console.WriteLine($"{Indent}Paged System Memory: {process.PagedSystemMemorySize64 / 1024} KB");
        Console.WriteLine($"{Indent}Peak Working Set: {process.PeakWorkingSet64 / (1024 * 1024)} MB");
        Console.WriteLine($"{Indent}Peak Virtual Memory: {process.PeakVirtualMemorySize64 / (1024 * 1024)} MB");

        // Memory retention and allocation pattern
        Console.WriteLine($"{Indent}GC Collection Counts:");
        Console.WriteLine($"{Indent}Gen 0: {GC.CollectionCount(0)}");
        Console.WriteLine($"{Indent}Gen 1: {GC.CollectionCount(1)}");
        Console.WriteLine($"{Indent}Gen 2: {GC.CollectionCount(2)}");

        // CPU information
        Console.WriteLine("CPU Information:");
        Console.WriteLine($"{Indent}Processor Count: {Environment.ProcessorCount}");
        Console.WriteLine($"{Indent}Is 64-bit Process: {Environment.Is64BitProcess}");
        Console.WriteLine($"{Indent}Is 64-bit OS: {Environment.Is64BitOperatingSystem}");

        // Runtime config summary
        // Console.WriteLine("Runtime Config Summary:");
        // Console.WriteLine(
        //     $"{Indent}Server GC: {GCSettings.IsServerGC} (Best for: {(GCSettings.IsServerGC ? "High throughput, multi-core systems" : "UI responsiveness, client apps")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Concurrent GC: {GCSettings.LatencyMode != GCLatencyMode.Batch} (Best for: {(GCSettings.LatencyMode != GCLatencyMode.Batch ? "Reduced pauses, responsive apps" : "Maximum throughput")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}RetainVM: {isEnabled} (Best for: {(isEnabled ? "Long-running server apps with stable memory usage" : "Optimal memory usage with variable workloads")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Globalization Invariant: {IsGlobalizationInvariant()} (Best for: {(IsGlobalizationInvariant() ? "Performance, reduced memory usage" : "Full globalization support")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Tiered Compilation: {IsTieredCompilationEnabled()} (Best for: {(IsTieredCompilationEnabled() ? "Faster startup, optimized performance" : "Consistent performance")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}ReadyToRun: {IsReadyToRunEnabled()} (Best for: {(IsReadyToRunEnabled() ? "Faster startup, optimized code" : "Standard JIT compilation")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}DateTime Precision: {GetDateTimePrecision()} ms (Best for: {(GetDateTimePrecision() < 1 ? "High precision time measurements" : "Standard time measurements")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}ThreadPool Min Worker Threads: {minWorkerThreads} (Best for: {(minWorkerThreads > 10 ? "High concurrency workloads" : "Low concurrency workloads")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}ThreadPool Max Worker Threads: {maxWorkerThreads} (Best for: {(maxWorkerThreads > 100 ? "High throughput applications" : "Moderate throughput applications")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}ThreadPool Min Completion Port Threads: {minCompletionPortThreads} (Best for: {(minCompletionPortThreads > 10 ? "High I/O workloads" : "Low I/O workloads")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}ThreadPool Max Completion Port Threads: {maxCompletionPortThreads} (Best for: {(maxCompletionPortThreads > 100 ? "High I/O throughput" : "Moderate I/O throughput")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Available Worker Threads: {availableThreads.Item1} (Best for: {(availableThreads.Item1 > 10 ? "High concurrency" : "Low concurrency")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Available Completion Port Threads: {availableThreads.Item2} (Best for: {(availableThreads.Item2 > 10 ? "High I/O concurrency" : "Low I/O concurrency")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Stopwatch Frequency: {Stopwatch.Frequency} ticks/second (Best for: {(Stopwatch.IsHighResolution ? "High precision timing" : "Standard timing")})"
        // );
        // Console.WriteLine(
        //     $"{Indent}Stopwatch IsHighResolution: {Stopwatch.IsHighResolution} (Best for: {(Stopwatch.IsHighResolution ? "Precise timing measurements" : "Standard timing")})"
        // );

        Console.WriteLine("=================================================");
    }

    /// <summary>
    /// Retrieves runtime information about the application and returns it as a formatted text string.
    /// </summary>
    /// <returns>
    /// A formatted string containing runtime information, such as the application's current runtime environment,
    /// configuration details, or system-specific data.
    /// </returns>
    public static string GetRuntimeInfoAsText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("========== .NET RUNTIME CONFIGURATION ==========");

        sb.AppendLine("Runtime Information:");
        sb.AppendLine($"{Indent}OS Version: {Environment.OSVersion}");
        sb.AppendLine($"{Indent}Framework Description: {Environment.Version}");
        sb.AppendLine($"{Indent}Machine Name: {Environment.MachineName}");
        sb.AppendLine($"{Indent}User Domain Name: {Environment.UserDomainName}");
        sb.AppendLine($"{Indent}User Name: {Environment.UserName}");
        sb.AppendLine($"{Indent}Processor Count: {Environment.ProcessorCount}");
        sb.AppendLine($"{Indent}Current Directory: {Environment.CurrentDirectory}");

        // Basic information
        sb.AppendLine($"{Indent}CLR Version: {Environment.Version}");
        sb.AppendLine($"{Indent}Framework Description: {RuntimeInformation.FrameworkDescription}");
        sb.AppendLine($"{Indent}Process Architecture: {RuntimeInformation.ProcessArchitecture}");
        sb.AppendLine($"{Indent}OS Description: {RuntimeInformation.OSDescription}");
        sb.AppendLine($"{Indent}OS Architecture: {RuntimeInformation.OSArchitecture}");

        // Runtime environment information
        sb.AppendLine("Environment Variables:");
        sb.AppendLine($"{Indent}DOTNET_GCHeapCount: {Environment.GetEnvironmentVariable("DOTNET_GCHeapCount") ?? ValueNotSet}");
        sb.AppendLine($"{Indent}DOTNET_GCCpuGroup: {Environment.GetEnvironmentVariable("DOTNET_GCCpuGroup") ?? ValueNotSet}");
        sb.AppendLine($"{Indent}DOTNET_ReadyToRun: {Environment.GetEnvironmentVariable("DOTNET_ReadyToRun") ?? ValueNotSet}");
        sb.AppendLine($"{Indent}DOTNET_TieredCompilation: {Environment.GetEnvironmentVariable("DOTNET_TieredCompilation") ?? ValueNotSet}");
        sb.AppendLine(
            $"{Indent}DOTNET_TC_QuickJitForLoops: {Environment.GetEnvironmentVariable("DOTNET_TC_QuickJitForLoops") ?? ValueNotSet}"
        );
        sb.AppendLine($"{Indent}Is Globalization Invariant: {IsGlobalizationInvariant()}");

        // GC information
        sb.AppendLine("GC Configuration:");
        sb.AppendLine($"{Indent}IsServerGC: {GCSettings.IsServerGC}");
        sb.AppendLine($"{Indent}GC Mode: {(GCSettings.IsServerGC ? "Server" : "Workstation")}");
        sb.AppendLine($"{Indent}LatencyMode: {GCSettings.LatencyMode}");
        sb.AppendLine($"{Indent}LargeObjectHeapCompactionMode: {GCSettings.LargeObjectHeapCompactionMode}");

        // RetainVM status
        var (isEnabled, source) = GetRetainVmStatus();
        sb.AppendLine($"{Indent}RetainVM: {isEnabled}");
        sb.AppendLine($"{Indent}RetainVM Source: {source}");

        // Detailed information about the number of GC heaps and cores
        var (estimatedHeapCount, threadsPerHeap) = GetDetailedGcInfo();
        sb.AppendLine($"{Indent}Estimated GC Heaps: {estimatedHeapCount}");
        sb.AppendLine($"{Indent}Total Processors: {Environment.ProcessorCount}");
        sb.AppendLine($"{Indent}GC Threads Per Heap: {threadsPerHeap}");

        try
        {
            sb.AppendLine($"{Indent}LOH Size: {GC.GetGCMemoryInfo().HeapSizeBytes / (1024 * 1024)} MB");
            sb.AppendLine($"{Indent}Total Available Memory: {GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024)} MB");

            if (Environment.Version.Major >= 5)
            {
                sb.AppendLine(
                    $"{Indent}High Memory Load Threshold: {GC.GetGCMemoryInfo().HighMemoryLoadThresholdBytes / (1024 * 1024)} MB"
                );
                sb.AppendLine($"{Indent}Total Committed Bytes: {GC.GetGCMemoryInfo().TotalCommittedBytes / (1024 * 1024)} MB");
                sb.AppendLine($"{Indent}Promoted Bytes: {GC.GetGCMemoryInfo().PromotedBytes / (1024 * 1024)} MB");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"Could not get GC Memory Info: {ex.Message}");
        }

        // Globalization configuration information
        sb.AppendLine("Globalization Configuration:");
        sb.AppendLine($"{Indent}CurrentCulture: {CultureInfo.CurrentCulture.Name}");
        sb.AppendLine($"{Indent}CurrentUICulture: {CultureInfo.CurrentUICulture.Name}");
        sb.AppendLine($"{Indent}InvariantCulture: {CultureInfo.InvariantCulture.Name}");

        // Time resolution and performance information
        sb.AppendLine("Performance Configuration:");
        sb.AppendLine($"{Indent}Stopwatch.Frequency: {Stopwatch.Frequency} ticks/second");
        sb.AppendLine($"{Indent}Stopwatch.IsHighResolution: {Stopwatch.IsHighResolution}");
        sb.AppendLine($"{Indent}DateTime.UtcNow Precision: {GetDateTimePrecision()} ms");

        // ThreadPool configuration information
        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
        var availableThreads = GetAvailableThreads();

        sb.AppendLine("ThreadPool Configuration:");
        sb.AppendLine($"{Indent}Min Worker Threads: {minWorkerThreads}");
        sb.AppendLine($"{Indent}Min Completion Port Threads: {minCompletionPortThreads}");
        sb.AppendLine($"{Indent}Max Worker Threads: {maxWorkerThreads}");
        sb.AppendLine($"{Indent}Max Completion Port Threads: {maxCompletionPortThreads}");
        sb.AppendLine($"{Indent}Available Worker Threads: {availableThreads.Item1}");
        sb.AppendLine($"{Indent}Available Completion Port Threads: {availableThreads.Item2}");

        // JIT information
        sb.AppendLine("JIT Configuration:");
        sb.AppendLine($"{Indent}Is Tiered Compilation Enabled: {IsTieredCompilationEnabled()}");
        sb.AppendLine($"{Indent}Is ReadyToRun Enabled: {IsReadyToRunEnabled()}");

        // Memory information
        var process = Process.GetCurrentProcess();
        sb.AppendLine("Process Memory:");
        sb.AppendLine($"{Indent}Working Set: {process.WorkingSet64 / (1024 * 1024)} MB");
        sb.AppendLine($"{Indent}Private Memory: {process.PrivateMemorySize64 / (1024 * 1024)} MB");
        sb.AppendLine($"{Indent}Virtual Memory: {process.VirtualMemorySize64 / (1024 * 1024)} MB");
        sb.AppendLine($"{Indent}Paged Memory: {process.PagedMemorySize64 / (1024 * 1024)} MB");
        sb.AppendLine($"{Indent}Paged System Memory: {process.PagedSystemMemorySize64 / 1024} KB");
        sb.AppendLine($"{Indent}Peak Working Set: {process.PeakWorkingSet64 / (1024 * 1024)} MB");
        sb.AppendLine($"{Indent}Peak Virtual Memory: {process.PeakVirtualMemorySize64 / (1024 * 1024)} MB");

        // Memory retention and allocation pattern
        sb.AppendLine($"{Indent}GC Collection Counts:");
        sb.AppendLine($"{Indent}Gen 0: {GC.CollectionCount(0)}");
        sb.AppendLine($"{Indent}Gen 1: {GC.CollectionCount(1)}");
        sb.AppendLine($"{Indent}Gen 2: {GC.CollectionCount(2)}");

        // CPU information
        sb.AppendLine("CPU Information:");
        sb.AppendLine($"{Indent}Processor Count: {Environment.ProcessorCount}");
        sb.AppendLine($"{Indent}Is 64-bit Process: {Environment.Is64BitProcess}");
        sb.AppendLine($"{Indent}Is 64-bit OS: {Environment.Is64BitOperatingSystem}");

        sb.AppendLine("=================================================");

        return sb.ToString();
    }

    // Helper methods
    /// <summary>
    /// Measures and calculates the smallest detectable precision of <see cref="DateTime.UtcNow"/> in milliseconds.
    /// This method samples the ticks from <see cref="DateTime.UtcNow"/> and determines the minimal interval
    /// between consecutive changes within a defined sampling period.
    /// </summary>
    /// <returns>
    /// The smallest precision of <see cref="DateTime.UtcNow"/> in milliseconds,
    /// or -1 if no sufficient data could be collected within the sampling period.
    /// </returns>
    private static double GetDateTimePrecision()
    {
        var samples = new List<long>();
        var sw = Stopwatch.StartNew();
        var lastTicks = DateTime.UtcNow.Ticks;

        // Measure the smallest change in DateTime.UtcNow
        while (samples.Count < 100 && sw.ElapsedMilliseconds < 1000)
        {
            var currentTicks = DateTime.UtcNow.Ticks;
            if (currentTicks == lastTicks)
            {
                continue;
            }

            samples.Add(currentTicks - lastTicks);
            lastTicks = currentTicks;
        }

        if (samples.Count == 0)
        {
            return -1;
        }

        // Convert the average tick count to milliseconds
        return samples.Min() * 1000.0 / Stopwatch.Frequency;
    }

    /// <summary>
    /// Retrieves the number of available worker threads and completion port threads
    /// from the thread pool.
    /// </summary>
    /// <returns>
    /// A <see cref="Tuple{T1, T2}"/> containing the number of available worker threads
    /// and completion port threads, in that order.
    /// </returns>
    private static Tuple<int, int> GetAvailableThreads()
    {
        ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
        return Tuple.Create(workerThreads, completionPortThreads);
    }

    /// <summary>
    /// Determines whether tiered compilation is enabled in the current runtime environment.
    /// </summary>
    /// <returns>
    /// <c>true</c> if tiered compilation is enabled; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsTieredCompilationEnabled()
    {
        var envVar = Environment.GetEnvironmentVariable("DOTNET_TieredCompilation");
        if (!string.IsNullOrEmpty(envVar))
        {
            return envVar.Equals("1") || envVar.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        // By default, tiered compilation is enabled from .NET Core 3.0
        return Environment.Version.Major >= 3;
    }

    /// <summary>
    /// Determines whether the ReadyToRun optimization feature is enabled based on the DOTNET_ReadyToRun environment variable.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the environment variable "DOTNET_ReadyToRun" is set to "1" or "true" (case-insensitive),
    /// or if the variable is not set, assuming ReadyToRun is enabled for published applications.
    /// Otherwise, returns <c>false</c>.
    /// </returns>
    private static bool IsReadyToRunEnabled()
    {
        var envVar = Environment.GetEnvironmentVariable("DOTNET_ReadyToRun");
        if (!string.IsNullOrEmpty(envVar))
        {
            return envVar.Equals("1") || envVar.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        // By default, ReadyToRun is enabled in published applications
        return true; // Assume it has been published
    }

    // Method to get detailed GC information
    /// <summary>
    /// Retrieves detailed garbage collection (GC) information, including the estimated number of GC heaps
    /// and the number of threads per heap, based on the current runtime GC configuration.
    /// </summary>
    /// <returns>
    /// A tuple containing:
    /// - <c>estimatedHeapCount</c>: An integer representing the estimated number of GC heaps.
    /// - <c>threadsPerHeap</c>: An integer indicating the number of threads per GC heap.
    /// </returns>
    private static (int estimatedHeapCount, int threadsPerHeap) GetDetailedGcInfo()
    {
        int heapCount;
        int threadsPerHeap;
        // Estimate the number of GC heaps based on configuration
        if (GCSettings.IsServerGC)
        {
            // In Server GC, there is usually one heap per logical processor (up to a limit)
            var heapCountEnv = Environment.GetEnvironmentVariable("DOTNET_GCHeapCount") ?? ValueNotSet;
            if (!string.IsNullOrEmpty(heapCountEnv) && int.TryParse(heapCountEnv, out var configuredHeapCount))
            {
                heapCount = configuredHeapCount;
            }
            else
            {
                // Default: The maximum number of heaps is the number of processors, but typically optimized to around 8 heaps
                heapCount = Math.Min(Environment.ProcessorCount, 8);
            }

            // Server GC has one dedicated thread per heap
            threadsPerHeap = 1;
        }
        else
        {
            // Workstation GC has one heap and uses foreground/background GC
            heapCount = 1;
            threadsPerHeap = GCSettings.LatencyMode == GCLatencyMode.Batch ? 1 : 2;
        }

        return (heapCount, threadsPerHeap);
    }

    /// <summary>
    /// Determines whether the current runtime is configured to use globalization-invariant mode.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the runtime is set to operate in globalization-invariant mode, either through
    /// the "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT" environment variable or through observed behavior;
    /// otherwise, <c>false</c>.
    /// </returns>
    private static bool IsGlobalizationInvariant()
    {
        // Check configuration
        var invariantEnv = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT") ?? ValueNotSet;
        if (!string.IsNullOrEmpty(invariantEnv))
        {
            return invariantEnv.Equals("1") || invariantEnv.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        // Or check by behavior
        try
        {
            return CultureInfo.CurrentCulture.Name == CultureInfo.InvariantCulture.Name
                && CultureInfo.CurrentUICulture.Name == CultureInfo.InvariantCulture.Name;
        }
        catch
        {
            return false;
        }
    }

    // Method to get information about the RetainVM configuration
    /// <summary>
    /// Retrieves the Retain VM status and its source.
    /// </summary>
    /// <returns>
    /// A tuple containing a boolean indicating if Retain VM is enabled and a string specifying the source of the status.
    /// </returns>
    private static (bool isEnabled, string source) GetRetainVmStatus()
    {
        // Check the environment variable first
        var envValue = Environment.GetEnvironmentVariable("DOTNET_GCRetainVM") ?? ValueNotSet;
        if (!string.IsNullOrEmpty(envValue))
        {
            var enabled = envValue == "1" || envValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            return (enabled, "Environment variable");
        }

        // Check registry or other configuration (example only, does not actually access registry)
        try
        {
            // This is just a simulation, in reality you could check the registry or other configuration
            // On Windows, you could check HKLM\SOFTWARE\Microsoft\.NETFramework\GCRetainVM

            // Since there is no direct API to check GCRetainVM, we estimate based on behavior
            // This is only an assumption - there is no exact way to determine if RetainVM is enabled
            // from the runtime unless using the environment variable

            // Check if runtimeconfig.template.json has System.GC.RetainVM = false
            // This is an assumption based on a known config file
            return (false, "runtimeconfig.template.json");
        }
        catch
        {
            // By default from .NET 5+, RetainVM is enabled
            return Environment.Version.Major >= 5
                ? (true, "Default (.NET 5+ default)")
                : (false, "Default (pre-.NET 5 default)");
        }
    }
}
