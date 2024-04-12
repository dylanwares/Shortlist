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

function fetchVotes(postId) {
    var popup = document.getElementById('votesPopup');
    popup.classList.add('overlay-active');
    popup.setAttribute("post", postId);

    var data = {
        post: postId
    }

    console.log(postId);

    $.ajax({
        type: "GET",
        url: "/Home/FetchVotes",
        contentType: "application/json",
        dataType: "json",
        data: { postId: postId },
        success: function (response) {
            console.log(response);
            displayVotes(JSON.parse(response.votes));
        }
    })
}

function displayVotes(votes) {
    var container = document.getElementById('votes');
    votes.forEach((vote) => {
        var voteNode = document.createElement("div");
        voteNode.style.flexDirection = "row";
        voteNode.style.columnGap = "5px";
        var voteIconNode = document.createElement("div");
        var nameNode = document.createElement("div");
        nameNode.style = "justify-content: center; font-size: 14px;"
        nameNode.innerHTML = vote.user.name.trim();
        if (vote.vote == 1) {
            voteIconNode.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24"><path fill="limegreen" d="M12 21c-1.654 0-3-1.346-3-3v-4.764c-1.143 1.024-3.025.979-4.121-.115a3.002 3.002 0 0 1 0-4.242L12 1.758l7.121 7.121a3.002 3.002 0 0 1 0 4.242c-1.094 1.095-2.979 1.14-4.121.115V18c0 1.654-1.346 3-3 3M11 8.414V18a1.001 1.001 0 0 0 2 0V8.414l3.293 3.293a1.023 1.023 0 0 0 1.414 0a.999.999 0 0 0 0-1.414L12 4.586l-5.707 5.707a.999.999 0 0 0 0 1.414a1.023 1.023 0 0 0 1.414 0z" /></svg>'
        }
        else if (vote.vote == -1){
            voteIconNode.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24"><path fill="red" d="m12 21.312l-7.121-7.121a3.002 3.002 0 0 1 0-4.242C5.973 8.855 7.857 8.811 9 9.834V5c0-1.654 1.346-3 3-3s3 1.346 3 3v4.834c1.143-1.023 3.027-.979 4.121.115a3.002 3.002 0 0 1 0 4.242zM7 11.07a.999.999 0 0 0-.707 1.707L12 18.484l5.707-5.707a.999.999 0 0 0 0-1.414a1.021 1.021 0 0 0-1.414 0L13 14.656V5a1.001 1.001 0 0 0-2 0v9.656l-3.293-3.293A.991.991 0 0 0 7 11.07" /></svg>'
        }
        container.appendChild(voteNode);
        voteNode.appendChild(voteIconNode);
        voteNode.appendChild(nameNode);
    })
}

function closeVotesPopup() {
    var popup = document.getElementById('votesPopup');
    popup.classList.remove('overlay-active');
    var node = document.getElementById('votes');
    while (node.firstChild) {
        node.removeChild(node.lastChild);
    }
}

function openInvitePopup() {
    var popup = document.getElementById('invitePopup');
    popup.classList.add('overlay-active');
}

function copyInviteLink() {
    var url = document.getElementById('inviteLinkInput').value;
    navigator.clipboard.writeText(url);
    document.getElementById('inviteLinkBut').innerHTML = 'Copied';
}