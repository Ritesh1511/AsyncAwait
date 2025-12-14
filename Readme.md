Async/await is a C# feature that lets us write asynchronous code without blocking the current thread.
It allows long-running operations (like file or data access) to run in the background using async keyword.
While await pauses the method (not the thread) until the operation completes, then continues execution.

Method starts on main thread
**Hits await ReadFileAsync()** 
- async (Code): Now this Read request sent to OS I/O
- Main thread is freed (What does main thread do then? **UI app**: Main thread keeps processing UI events (clicks, paint, input). **ASP.NET / server**: Thread goes back to thread pool to serve other requests. or remain idle.)
- OS finishes I/O operations
- It will notify .NET runtime.
- Now if we don't used await or .result main thread will (exit the program in console application) and (request ends before async work finishes).
- **await** - still await frees the thread; the OS independently handles the I/O; no thread is “given” to the OS.
- **.result** - it will block main thread.
- Either of these must be used to recieve async response.
**This way main thread never blocked and always there to cater ui events or idle in threadpool.**

##To mark a method async
**async method can only return Task or Task<T>**
**Task returns no value and Task<T> is generic and returns the type of placeholder**

```
public async Task WaitAndPrintAsync()
{
    await Task.Delay(1000); // wait 1 second
    Console.WriteLine("Done waiting!");
}
```

```
public async Task<string> GetGreetingAsync()
{
    await Task.Delay(500); // simulate async work
    return "Hello, async!";
}

```
```await Task1Async(); // wait for just one task```
```
var t1 = Task1Async();
var t2 = Task2Async();
await Task.WhenAll(t1, t2); // wait until all complete
```
```
var t1 = Task1Async();
var t2 = Task2Async();
var first = await Task.WhenAny(t1, t2); // resumes when the first finishes
```
```
await Task.Delay(1000); // wait 1 second without blocking
```



## 🔁 .Result vs await

### ❌ Blocking example (bad for UI/server apps)

```csharp
public static void Main2()
{
    Console.WriteLine("Starting...");
    var content = File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
    Console.WriteLine(content.Result); // Blocks thread! -- Main thread blocked here
    Console.WriteLine("Done");
}
```

- `.Result` blocks the current thread.
- Consumes thread while waiting (synchronous wait).
- Can cause unresponsiveness or thread starvation in UI/server apps.

---

### ✅ Non-blocking example (recommended)

```csharp
public static async Task Main3()
{
    Console.WriteLine("Starting...");
    var content = await File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
    Console.WriteLine(content);
    Console.WriteLine("Done");
}
```
**
await unwraps the Task and gives the result automatically in async methods;
await unwraps the Task and gives the result automatically in async methods;
await unwraps the Task and gives the result automatically in async methods;**

- Uses `await` — does **not block** the main thread.
- Async I/O operation is handled by the OS.
- OS notifies the .NET runtime when complete via callback.
- Main thread is free to serve other tasks (scalable, responsive).

**
File.ReadAllTextAsync is asynchronous and handled by OS I/O, not the main thread.

It immediately returns a Task<string>, not the actual file content.

content holds the Task, not the string, so:

Console.WriteLine(content); 


prints something like System.Threading.Tasks.Task — not the file content.

Even if you add Task.Delay(1000).Wait(), it doesn’t automatically extract the result from the Task. The main thread just waits 1 second, but content is still a Task.**


---

## ⏳ Task.Delay Comparison

```
public static void Main2()
    {
        Console.WriteLine("Starting...");
        Task.Delay(1000);
        Console.WriteLine("Done");
        //Task.Delay uses the OS/timer to track time asynchronously; main thread is free and not blocked.
    }

```
**Task.Delay itself doesn’t block; without await or .Wait(), a non-async method will just keep running**

**.Wait() pauses the current thread until the task completes**

### Task.Delay() alone - without await or wait() has no use : await free the main thread / wait blocks the current thread.

### ❌ Task.Delay without `await` — no effect

```csharp
public static async Task Main4()
{
    Console.WriteLine("Starting..."); 
    Task.Delay(120); // No effect Task.Delay uses the OS/timer to track time asynchronously; main thread is free and not blocked.
    Console.WriteLine("Finished");
}
```

- `Task.Delay()` runs in background — won’t block or delay without `await`.

---

### ✅ Task.Delay with `await` — works as intended

```csharp
public static async Task Main5()
{
    Console.WriteLine("Starting...");
    await Task.Delay(120); // Releases thread during wait
    Console.WriteLine("Finished");
}
```

- `await Task.Delay(...)` does not block a thread — uses a timer internally.
- Resumes execution after timeout.


### There is catch 
### There is catch
### There is catch

```
namespace asyncawait;

class Program
{
    static void Main(string[] args)
    {
        Main2();
    }
    public static async Task Main2()
    {
        Console.WriteLine("Starting..."); //Only this will be printed 
         await Task.Delay(100);          //main thread is free and returns to the caller 
        Console.WriteLine("Done");
    }

}
```
### At await, the main thread returns to the caller while the async operation runs, and the method resumes later on a thread when the task completes.


### So to print all caller must await.

```
namespace asyncawait;

class Program
{
    static async Task Main(string[] args)
    {
        await  Main2();
    }
    public static async Task Main2()
    {
        Console.WriteLine("Starting...");
        await Task.Delay(1000);
        Console.WriteLine("Done");
    }

}
```


### There is catch 
### There is catch
### There is catch



---

### ❌ Task.Delay().Wait() — blocking

```
csharp
public static void Main6()
{
    Console.WriteLine("Starting...");
    Task.Delay(120).Wait(); // Blocks thread!
    Console.WriteLine("Finished");
}
```

- `Wait()` is synchronous and blocks the current thread.
- Should be avoided in async applications.




---




## 📂 Parallel File Reads

```csharp
public static async Task Main7()
{
    var task1 = File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
    var task2 = File.ReadAllTextAsync("/Users/riteshkaushik/unused_files copy.txt");
    var task3 = File.ReadAllTextAsync("/Users/riteshkaushik/package.json");

    var result = await Task.WhenAll(task1, task2, task3);

    Console.WriteLine(result[0]);
    Console.WriteLine(result[1]);
    Console.WriteLine(result[2]);
}
```

- Starts all 3 file reads in parallel.
- `await Task.WhenAll(...)` waits for all tasks to complete.
- Efficient and non-blocking.

---

## 🧵 Task.Run with ThreadPool

```csharp
public static async Task Main8()
{
    var t1 = Task.Run(() => { Search("/Users/riteshkaushik/Downloads"); });
    var t2 = Task.Run(() => { Search("/Users/riteshkaushik/Projects"); });
    var t3 = Task.Run(() => { Search("/Users/riteshkaushik/Document"); });

    await Task.WhenAll(t1, t2, t3);
}
```

- `Task.Run` queues work to a **.NET ThreadPool thread**.
- Threads are scheduled by the **OS**.
- `await Task.WhenAll(...)` ensures all threads complete before proceeding.

### 🔥 Key Note:

```csharp
await Task.Run(() => Search(...));
```

Running tasks **with await one-by-one** like this would make them run **serially** instead of in parallel.

---

## 🔍 File Search Helper

```csharp
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
```

### Task.Run → uses thread-pool threads, CPU executes code

### File.ReadAllTextAsync → uses OS async I/O, main/thread-pool threads are mostly free


### Task type                     	         How to run
I/O-bound (network, disk, DB)      	     await asyncMethod()
CPU-bound (computation, heavy loops)	 Task.Run(() => work())

---

## 🧠 Summary

- `await` releases the thread while waiting for I/O.
- OS handles async I/O and notifies .NET upon completion.
- `Task.Run` creates background threads from the thread pool.
- Use `await Task.WhenAll(...)` to run tasks in parallel properly.
- Avoid `.Result`, `.Wait()`, or other blocking patterns in async code.

---

📂 **Project structure**
- `Program.cs` – Demo code with all examples
- `Readme.md` – You're here 🙂
- `.csproj` – Project metadata








