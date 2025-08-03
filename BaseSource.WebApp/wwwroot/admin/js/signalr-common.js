
// signalr
var token = Cookies.get("Token");
var connectionUser = new signalR.HubConnectionBuilder()
    .withUrl(URL_API + "/commonHub", { accessTokenFactory: () => token })
    .withAutomaticReconnect()
    .build();

connectionUser.on("UserBalanceChanged", function (data) {
    $('[data-binding="balance"]').html(data.Balance.toLocaleString("en"));
});

connectionUser.on("ReceiveMessage", function (message) {
    toastr.info(message);
});

connectionUser.start().then(function () {
    // do something
    //connectionUser.invoke("UpdateInfo", 1, 2, 3).catch(function (err) {
    //    return console.error(err.toString());
    //});
}).catch(function (err) {
    return console.error(err.toString());
});
///

function formatNumber(price) {
    if (price === null || price === undefined) {
        return ''; 
    }
    return price.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 4 });
}