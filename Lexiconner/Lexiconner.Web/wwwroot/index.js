

document.addEventListener("DOMContentLoaded", function (event) {
    httpGet('/config', function(config) {
        start(config);
    });
});


function start(config) {

    var counter = -1;
    var offset = 0;
    var limit = 5;
    var pages = 0;
    var cardData = {};

    function getTestData(countOfElmenets) {
        var arrData = [];
        for (var i = 0; i < countOfElmenets; i++) {
            if (i % 3 === 0) {
                arrData.push({
                    title: "title asdaaaa aaaa aaaaa aaaaa aaaaaaaaaa aaaaa asd asd asd asd asdasd asfnasdf sad f" + i,
                    description: " itle asdaaaa aaaa aaaaa aaaaa aaaaaaaaaa aaaaa asd asd asd asd asdasd asfnasdf sad  " + i,
                    example: "itle asdaaaa aaaa aaaaa aaaaa aaaaaaaaaa aaaaa asd asd asd asd asdasd asfnasdf sad  " + i
                });
            } else {
                arrData.push({
                    title: "title " + i,
                    description: " description " + i,
                    example: "example " + i
                });

            }
        }
        return arrData;
    }
    // /config
    // {urls: {api: '........'}}
    function getData(offset = 0, limit = 2, callBack = null) {
        httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
            console.log(2, data);
            if (callBack !== null) {
                callBack(data);
            }
        });
    }

    function showNextCard(direction = 1) {
        counter = counter + direction * 1;
        
        if (!cardData.items || (counter >= cardData.items.length || counter < 0)) {
            if(cardData.items && direction === -1) {
                offset = offset - limit;
                offset = offset < 0 ? ((pages - 1) * limit): offset;
            }
            if(cardData.items && direction === 1) {
                offset = offset + limit;
                offset = offset > cardData.totalCount ? 0: offset;
            }
            getData(offset, limit, function (response) {
                cardData = response.data;
                counter = direction === 1 ? 0 : cardData.items.length - 1;
               
                pages = Math.ceil(cardData.totalCount / limit);
                showDataOnCard(cardData.items[counter]);
            });
            

        } else {
            showDataOnCard(cardData.items[counter]);
        }
    }

    function showDataOnCard(card) {
        var elem = document.getElementById("cardBlock");
        var newTitle = document.createElement('div');
        var newDescription = document.createElement('div');
        var newExample = document.createElement('div');

        newTitle.className = "title";
        newTitle.innerHTML = card.title;

        newDescription.className = " description";
        newDescription.innerHTML = card.description || '';

        newExample.className = "example";
        newExample.innerHTML = card.exampleText || '';

        elem.replaceChild(newTitle, elem.childNodes[0]);
        elem.replaceChild(newDescription, elem.childNodes[1]);
        elem.replaceChild(newExample, elem.childNodes[2]);
    }

    var leftButtonEl = document.getElementById("cardButtonLeft");
    var rightButtonEl = document.getElementById("cardButtonRight");

    leftButtonEl.addEventListener("click", function (e) {
        showNextCard(-1);
    });

    rightButtonEl.addEventListener("click", function (e) {
        showNextCard(1);
    });

    function httpGet(url, callBack) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url);
        xhr.send();

        xhr.onload = function () {
            if (xhr.status === 200) {
                //console.log('User\'s name is ' + xhr.responseText);
                var data = JSON.parse(xhr.responseText);
                callBack(data);
                console.log(data);
            }
            else {
                console.error('Request failed.  Returned status of ' + xhr.status);
            }
        };
    }

    showNextCard();
}