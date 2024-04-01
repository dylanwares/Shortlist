var random1;
var random2;

function shortlistClicked(id) {
    window.location.href = 'shortlist?s=' + id;
}

function openPopup() {
    var popup = document.getElementById('popupOverlay');
    popup.classList.add('overlay-active');
    randomiseThumbnail();
}

function closePopup() {
    var popup = document.getElementById('popupOverlay');
    popup.classList.remove('overlay-active');
}

function createShortlist() {
    var but = document.getElementById('createShortlistBut');
    but.classList.add('disabled');
    but.innerHTML = 'Creating your shortlist...';

    var data = {
        name: document.getElementById('shortlistName').value,
        description: document.getElementById('shortlistDesc').value,
        primaryColour: random1,
        secondaryColour: random2
    };

    $.ajax({
        type: "POST",
        url: "/Home/CreateShortlist",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            window.location.href = 'shortlist?s=' + response.id;
        },
        error: function (error) {
            alert('There was an error creating your shortlist. Please try again.');
            but.classList.remove('disabled');
            but.innerHTML = 'Create Shortlist';
        }
    })
}

function randomiseThumbnail() {
    random1 = Math.floor(Math.random() * 5);
    random2 = Math.floor(Math.random() * 5);

    switch (random1) {
        case 0:
            first = '255, 152, 67';
            break;
        case 1:
            first = '66, 50, 168';
            break;
        case 2:
            first = '64, 168, 50';
            break;
        case 3:
            first = '207, 42, 33';
            break;
        case 4:
            first = '134, 167, 252';
            break;
        default:
    }

    switch (random2) {
        case 0:
            second = '255, 152, 67';
            break;
        case 1:
            second = '66, 50, 168';
            break;
        case 2:
            second = '64, 168, 50';
            break;
        case 3:
            second = '207, 42, 33';
            break;
        case 4:
            second = '134, 167, 252';
            break;
    }

    var tn = document.getElementById('createShortlistTN');
    tn.style.background = 'linear-gradient(to bottom right, rgb(' + first + '), rgb(' + second + '))';

    if (random1 == random2) {
        randomiseThumbnail();
    }
}

