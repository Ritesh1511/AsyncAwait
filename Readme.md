# 📘 AsyncAwait in C#

This project demonstrates how asynchronous programming works in C# using `async`, `await`, `Task.Run`, and `Task.WhenAll`.

It explains:
- How the OS and .NET handle async I/O
- The difference between blocking and non-blocking code
- How thread pool threads work in background execution

---

## 🏁 Entry Point

```csharp
static async Task Main(string[] args)
{
    await Main8();
}
```

---

## 🔁 Blocking vs Non-Blocking

### ❌ Blocking example (bad for UI/server apps)

```csharp
public static void Main2()
{
    Console.WriteLine("Starting...");
    var content = File.ReadAllTextAsync("/Users/riteshkaushik/unused_files.txt");
    Console.WriteLine(content.Result); // Blocks thread!
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

- Uses `await` — does **not block** the main thread.
- Async I/O operation is handled by the OS.
- OS notifies the .NET runtime when complete via callback.
- Main thread is free to serve other tasks (scalable, responsive).

---

## ⏳ Task.Delay Comparison

### ❌ Task.Delay without `await` — no effect

```csharp
public static async Task Main4()
{
    Console.WriteLine("Starting..."); 
    Task.Delay(120); // No effect
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

---

### ❌ Task.Delay().Wait() — blocking

```csharp
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
