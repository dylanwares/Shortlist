function signup() {
    if (document.getElementById('username').value == '' || document.getElementById('password').value == '') {
        alert('Please fill in all inputs.')
        return;
    }

    const signupBut = document.getElementById('signupBut');
    signupBut.style.pointerEvents = 'none';
    signupBut.innerHTML = 'Signing up...';
    signupBut.style.opacity = 0.5;

    var data = {
        name: document.getElementById('name').value,
        username: document.getElementById('username').value,
        password: document.getElementById('password').value
    }

    $.ajax({
        type: "POST",
        url: "/Home/CreateUser",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            window.location.href = 'dashboard';
        },
        error: function (error) {
            alert('That username is already in use. Please try again.');
            signupBut.style.pointerEvents = 'initial';
            signupBut.innerHTML = 'Sign up';
            signupBut.style.opacity = 1;
        }
        })
}
