namespace Grains

open System
open System.Threading
open System.Threading.Tasks
open Orleans

type IYourReminderGrain =
    inherit IGrainWithStringKey
    inherit IRemindable
    
    abstract member WakeUp : unit -> Task

type YourReminderGrain() =
    inherit Grain()
    interface IYourReminderGrain with
        member this.WakeUp() = Task.CompletedTask
        member this.ReceiveReminder(reminderName, status) =
            Console.WriteLine("reminderName: {0}", reminderName)
            Console.WriteLine("status: {0}", status)
            Task.CompletedTask
        
    override _.OnActivateAsync(cancellationToken:CancellationToken) = 
        let _periodInSeconds = TimeSpan.FromSeconds 60
        let _timeInSeconds = TimeSpan.FromSeconds 60
        let _reminder = base.RegisterOrUpdateReminder(base.GetPrimaryKeyString(), _periodInSeconds, _timeInSeconds)
        base.OnActivateAsync(cancellationToken)
        