namespace DunDatApi.Data;

public static class Tasks
{
    public static async Task<(T1, T2)> When<T1, T2>(Task<T1> task1, Task<T2> task2)
    {
        await Task.WhenAll(task1, task2);
        return (task1.Result, task2.Result);
    }
}