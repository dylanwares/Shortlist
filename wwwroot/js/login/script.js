function login() {
    const loginBut = document.getElementById('loginBut');
    loginBut.innerHTML = 'Logging in...';
    loginBut.style.opacity = 0.5;
    loginBut.style.pointerEvents = 'none';

    var data = {
        username: document.getElementById('username').value,
        password: document.getElementById('password').value
        }

    $.ajax({
        type: "POST",
        url: "/Home/Login",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            window.location.href = 'dashboard';
        },
        error: function (error) {
            alert('Email or password incorrect.');
            document.getElementById('loginBut').innerHTML = "Log in"
            loginBut.style.opacity = 1;
            loginBut.style.pointerEvents = 'initial';
        }
    })
}