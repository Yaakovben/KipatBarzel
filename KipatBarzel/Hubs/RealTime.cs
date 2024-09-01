using Microsoft.AspNetCore.SignalR;

namespace KipatBarzel.Hubs
{
    public class RealTime : Hub
    {
        // פונקציה לשליחת התראה בזמן אמת לכל הלקוחות המחוברים
        public async Task AttackAlert(int Id, int ResponseTime, string Name)
        {
            // ResponcTime, Name, Id  :עם הפרמטרים  SendAsync שליחת הודעה לכל הלקוחות דרך המתודה 
            await Clients.All.SendAsync("RedAlert", Id, ResponseTime, Name);
        }


        //public async Task SendMessage(string user, string message)
        //{
        //	await Clients.All.SendAsync("AttackAlert", user, message);
        //}
    }
}
