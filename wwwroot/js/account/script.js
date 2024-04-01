function logout() {
    $.ajax({
        type: "POST",
        url: "/Home/Logout",
        success: function (response) {
            window.location.href = './';
        }
    })
}
