function showLinkPost() {
    document.getElementById('textTabBut').classList.remove('activeTab');
    document.getElementById('linkTabBut').classList.add('activeTab');

    document.getElementById('linkPost').style.display = 'flex';
    document.getElementById('textPost').style.display = 'none';
}

function showTextPost() {
    document.getElementById('textTabBut').classList.add('activeTab');
    document.getElementById('linkTabBut').classList.remove('activeTab');
    document.getElementById('textPost').style.display = 'flex';
    document.getElementById('linkPost').style.display = 'none';
}

function linkPostChanged() {
    if (document.getElementById('linkTitle').value != '' && document.getElementById('linkInput').value != '') {
        document.getElementById('linkPostBut').classList.remove('disabled');
    }
    else {
        document.getElementById('linkPostBut').classList.add('disabled');
    }
}

function textPostChanged() {
    if (document.getElementById('textTitle').value != '') {
        document.getElementById('textPostBut').classList.remove('disabled');
    }
    else {
        document.getElementById('textPostBut').classList.add('disabled');
    }
}

function PostLink() {
    document.getElementById('linkPostBut').classList.add('disabled');
    document.getElementById('linkPostBut').innerHTML = 'Posting...';

    var data = {
        title: document.getElementById('linkTitle').value,
        body: document.getElementById('linkInput').value,
        isLink: true,
        shortlist: parseInt(new URLSearchParams(window.location.search).get('s'))
    };

    Post(data);
}

function PostText() {
    document.getElementById('textPostBut').classList.add('disabled');
    document.getElementById('textPostBut').innerHTML = 'Posting...';

    var data = {
        title: document.getElementById('textTitle').value,
        body: document.getElementById('textBody').value,
        isLink: false,
        shortlist: parseInt(new URLSearchParams(window.location.search).get('s'))
    };

    Post(data);
}

function Post(data) {
    $.ajax({
        type: "POST",
        url: "/Home/CreatePost",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            window.location.href = 'shortlist?s=' + new URLSearchParams(window.location.search).get('s');
        },
        error: function (error) {
            if (error.issue == 'badlink') { alert('There was an error fetching data from your link. Please ensure your link is correct.') }
            else { alert('There was an error creating your post. Please try again.'); }

            document.getElementById('linkPostBut').classList.remove('disabled');
            document.getElementById('linkPostBut').innerHTML = 'Post';
            document.getElementById('textPostBut').classList.remove('disabled');
            document.getElementById('textPostBut').innerHTML = 'Post';
        }
    })
}

function fetchImage() {
    $.ajax({
        type: "GET",
        url: "/Home/FetchLinkImage",
        data: { link: document.getElementById('linkInput').value },
        dataType: "json",
        success: function (res) {
            return res.img;
        },
        error: function (error) {
            alert('There was an error fetching data from your link. Please ensure your link is correct.');
        }
        })
}