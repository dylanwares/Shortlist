function openLink(link) {
    window.open(link, '_blank').focus();
}

function vote(postId, userId, vote, event) {
    event.target.style.pointerEvents = "none";
    var currentVote = parseInt(document.getElementById("vote-" + postId).getAttribute("vote"));

    var data = {
        post: postId,
        userId: userId,
        vote: vote,
        currentVote: currentVote
    }

    var voteCountElement = document.getElementById("voteCount-" + postId);
    var upvoteElement = document.getElementById("upvote-" + postId);
    var downvoteElement = document.getElementById("downvote-" + postId);

    if (vote == currentVote) {
        var voteCount = parseInt(voteCountElement.innerHTML) - currentVote;
        voteCountElement.innerHTML = voteCount;

        $.ajax({
            type: "POST",
            url: "/Home/DeleteVote",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (response) { },
            error: function (error) {
                alert("There was an error removing your vote. Please try again.");
                return;
            }
        })

        document.getElementById("vote-" + postId).setAttribute("vote", "0");
        upvoteElement.classList.remove("voted", "disabled")
        downvoteElement.classList.remove("voted", "disabled")
    }

    else {
        var voteCount = parseInt(voteCountElement.innerHTML) + vote;
        voteCountElement.innerHTML = voteCount;

        $.ajax({
            type: "POST",
            url: "/Home/Vote",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (response) {
                event.target.classList.add("voted");
            },
            error: function (error) {
                alert("There was an error adding your vote. Please try again.")
            }
        })

        if (vote == 1) {
            upvoteElement.classList.add("voted");
            downvoteElement.classList.add("disabled");
        }
        else if (vote == -1) {
            upvoteElement.classList.add("disabled");
            downvoteElement.classList.add("voted");
        }
        document.getElementById("vote-" + postId).setAttribute("vote", vote);
    }

    event.target.style.pointerEvents = null;
}

function removeVote(postId, userId, event) {
    var but = event.target;
    but.style.pointerEvents = "none";

    var data = {
        post: postId,
        userId: userId
        }

    $.ajax({
        type: "POST",
        url: "/Home/DeleteVote",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            but.classList.remove("voted");
            but.style.pointerEvents = "none";
            console.log(but.classList)
            if (but.classList.contains("upvote")) {
                but.onclick = "vote(", postId, ",", userId, ",1,event)";
            }
        },
        error: function (error) {
            alert("There was an error removing your vote. Please try again.");
            event.target.style.pointerEvents = "none";
            return;
        }
    })
}

function joinShortlist(userId, shortlistId, event) {
    event.target.classList.add("disabled");
    event.target.innerHTML = "Joining...";

    var data = {
        userId: userId,
        shortlist: shortlistId,
    }

    $.ajax({
        type: "POST",
        url: "/Home/JoinShortlist",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            window.location.reload();
        }
    })
}

function closeCommentsPopup() {
    var popup = document.getElementById('popupOverlay');
    popup.classList.remove('overlay-active');
    var node = document.getElementById('comments');
    while (node.firstChild) {
        node.removeChild(node.lastChild);
    }
}

function openComments(postId) {
    var popup = document.getElementById('popupOverlay');
    popup.classList.add('overlay-active');
    popup.setAttribute("post", postId);

    var data = {
        post: postId
    }

    $.ajax({
        type: "GET",
        url: "/Home/FetchComments",
        contentType: "application/json",
        dataType: "json",
        data: {postId: postId},
        success: function (response) {
            displayComments(JSON.parse(response.comments));
        }
    })
}

function postComment(name) {
    var data = {
        body: document.getElementById('commentInput').value,
        post: parseInt(document.getElementById('popupOverlay').getAttribute('post'))
    }

    console.log(data);

    $.ajax({
        type: "POST",
        url: "/Home/CreateComment",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            document.getElementById('commentInput').value = '';
            var container = document.getElementById('comments');
            var commentNode = document.createElement("div");
            var nameNode = document.createElement("div");
            var bodyNode = document.createElement("div");
            nameNode.classList.add("commentName");
            bodyNode.classList.add("commentBody");
            nameNode.innerHTML = name;
            bodyNode.innerHTML = data.body;
            container.insertBefore(commentNode, container.firstChild);
            commentNode.appendChild(nameNode);
            commentNode.appendChild(bodyNode);
        },
        error: function (error) {
            alert('There was an error creating your comment.');
        }
    })
}

function displayComments(comments) {
    console.log(comments);
    var container = document.getElementById('comments');
    comments.forEach((comment) => {
        var commentNode = document.createElement("div");
        var nameNode = document.createElement("div");
        var bodyNode = document.createElement("div");
        nameNode.classList.add("commentName");
        bodyNode.classList.add("commentBody");
        nameNode.innerHTML = comment.user.name.trim();
        bodyNode.innerHTML = comment.body.trim();
        container.appendChild(commentNode);
        commentNode.appendChild(nameNode);
        commentNode.appendChild(bodyNode);
    })
}

function openMembersPopup() {
    var popup = document.getElementById("membersPopupOverlay");

    popup.classList.add('overlay-active')
}

function closePopup() {
    var popup = document.getElementsByClassName('overlay-active')[0];
    popup.classList.remove('overlay-active');
}

function openPostOptionsPopup(post) {
    var popup = document.getElementById('postOptionsPopup');
    popup.classList.add('overlay-active');
    popup.setAttribute('post', post);
}

function deletePost() {
    var data = {
        post: document.getElementById('postOptionsPopup').getAttribute('post')
    }

    $.ajax({
        type: "POST",
        url: "/Home/DeletePost",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (response) {
            window.location.reload();
        },
        error: function (error) {
            alert('There was an error deleting your post.');
        }
    })
}