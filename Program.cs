namespace AsyncAwait;

class Program
{
    static async Task Main(string[] args)
    {
        await Main8();
        
    }
    
    public static void Main2()
    {
        Console.WriteLine("Starting...");
        var content = File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
        Console.WriteLine(content.Result);
        Console.WriteLine("Done");
    }
    
   public static async Task Main3()
    {
        Console.WriteLine("Starting...");
        var content =  await File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
        Console.WriteLine(content);
        Console.WriteLine("Done");
    }
   
   //  ** When you await an asynchronous I/O operation, the OS handles the operation in the 
   //     background (without blocking a thread), and then 
   // notifies the .NET runtime via a callback when it's complete — at which point your method resumes.
   
   
   
    // Both methods are same doing same thing 
   
    //  Main2 blocks the current thread using .Result.
    //     *    - This is a synchronous wait.
    //     *    - It consumes a thread while waiting.
    //     *    - In a UI or server app, this can cause unresponsiveness or thread starvation.
    //  File.ReadAllTextAsync itself is Async function and return Task<string>
    // This `Task<string>` represents an operation that will complete in the future with the file contents.
   
    
    // Main3 will not block the main thread - Because using await a new thread
    // from thread pool picked this task managed by OS and will return the output to .NET Runtime 
    // using callback.
    // No thread is held during the wait.
    // Making app more scalable.
    
    // Means main thread will be free and can serve other request from the UI which will
    // eventually make UI more smoother.
    
    //  Summary:
    // *   - Main2: Blocking (bad for UI/server)
    // *   - Main3: Non-blocking (recommended for async apps)

    public static async Task Main4()
    {
        Console.WriteLine("Starting..."); 
        Task.Delay(120); // No effect won't work because timeout will run in background and won't affect thread
        Console.WriteLine("Finished");
        
    }
    public static async Task Main5()
    {
        Console.WriteLine("Starting...");
        await Task.Delay(120); // Now this will work because of await
        Console.WriteLine("Finished");
        
    }
    public static void Main6()
    {
        Console.WriteLine("Starting...");
         Task.Delay(120).Wait();   //wait cannot be used with await because wait means blocking thread and await means releasing thread.
        Console.WriteLine("Finished");
        
    }
    
    // await Task.Delay(120); ==  Task.Delay(120).Wait();
    // just that await Task.Delay(120); it will release main thread while wait will block the thread.
    // Task.Delay(120) this function won't require any thread, it can complete by itself in background.
    
    // Task.Delay(120) - It will create background timeout in .NET Runtime
    // If used alone this will run and complete in background - no effect 
    
    //  Task.Delay(120).Wait(); -- This cannot be used with await
    
    
    
    public static async Task Main7()
    {
        var task1 =  File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
        var task2 =  File.ReadAllTextAsync("/Users/riteshkaushik/unused_files copy.txt");
        var task3 =  File.ReadAllTextAsync("/Users/riteshkaushik/package.json");
        
       var result  =  await Task.WhenAll(task1, task2, task3);
       
       Console.WriteLine(result[0]);
       Console.WriteLine(result[1]);
       Console.WriteLine(result[2]);
       
    }
    
    // var task1,2,3 these do not block the main thread. Task is handled by Async IO calls by OS.  
    // Again at await - Async IO calls go to os and it take care of it here main thread



    
    // Task.Run queues work to a thread pool thread,
    // which is an OS thread managed by the OS scheduler, to run your code asynchronously.
    public static async Task Main8()
    {
         var t1 =  Task.Run(() => { Search("/Users/riteshkaushik/Downloads"); });

         var t2 = Task.Run(() => { Search("/Users/riteshkaushik/Projects"); });

         var t3 = Task.Run(() => { Search("/Users/riteshkaushik/Document"); });
         
         await Task.WhenAll(t1, t2, t3);
    }

// await Task.Run(() => { Search("/Users/riteshkaushik/Downloads"); });
//
// await Task.Run(() => { Search("/Users/riteshkaushik/Projects"); });
//
// await Task.Run(() => { Search("/Users/riteshkaushik/Document"); });

// Above does not make sense because await will wait for first to finish and then all will run in series.


// ********************

//   Task.Run(() => { Search("/Users/riteshkaushik/Downloads"); });
//
//   Task.Run(() => { Search("/Users/riteshkaushik/Projects"); });
//
//   Task.Run(() => { Search("/Users/riteshkaushik/Document"); });

    // ** Task.Run queues work to a thread pool thread,
    // ** which is an OS thread managed by the OS scheduler, to run your code asynchronously.
    
//  These tasks are getting run by new thread and that new thread does not await for newly created threads because 
// we have'nt used awaited statement so even though they are running in background still main thread ends the program.
// If we somehow block the main thread these above threads gets completed and return their output.

//Because main thread stops at main function and OS handles Main8
    //     The main thread is freed at the await of Main8.
    //     The OS manages scheduling of thread pool threads running your 3 tasks.
    //     Your tasks run concurrently on separate threads, managed by .NET’s thread pool and OS.
    public static void Search(string input)
    {
        if (!Directory.Exists(input))
        {
            Console.WriteLine($"Directory not found: {input}");
            return;
        }
        foreach (var d in Directory.GetDirectories(input))
        {
            foreach (var file in Directory.GetFiles(d))
            {
            
                Console.WriteLine(file);
            }
        }
    }
    
    
}



 

