// SignalR Hub יצירת חיבור עם
const connection = new signalR.HubConnectionBuilder().withUrl("/rt").build()

// התחלת חיבור
connection.start()
    .then(function () {
        // ביצוע פעולה כלשהי לאחר התחברות מוצלחת
    })
    .catch(function (err) {
        // במקרה של שגיאה הדפסת השגיאה למסוף
        return console.error(err.toString());
    });


// פונקציה לקריאה להתקפה
function invokeLaunch(Id, ResponceTime, Name) {
    // שליחת פעולה 
    connection.invoke("AttackAlert", Id,ResponseTime, Name)
    console.log("I am inisde the launch invoke func")
}

// invoke inretcept

// listen to lauch
connection.on("RedAlert", function (Id, ResponseTime, Name) {
    if (window.location.href.includes("Deffence")) return;
    const h1 = document.createElement("h1")
    h1.style.color = "red"
    h1.textContent = Name + " has sent you a present! it'll arraive in " + ResponseTime + " seconds"
    document.body.appendChild(h1)
    setTimeout(() => {
        document.body.removeChild(h1)
    }, 5000)

})

// listen to intercept
