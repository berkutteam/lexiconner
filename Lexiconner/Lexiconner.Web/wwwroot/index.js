

document.addEventListener("DOMContentLoaded", function (event) {
    start();
});


function start() {

    var counter = 0;

    function GetTestData(countOfElmenets) {
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

    function GetData(callBack2) {

        httpGet('http://localhost:61414/api/v1/studyitems/', function (data) {
            console.log(2, data);
            callBack2(data);
        });

        // return GetTestData(countOfElmenets);
    }

    function ShowDataOnCard() {

        GetData(function (data) {

            var item = data.data[counter];

            var elem = document.getElementById("cardBlock");
            var newTitle = document.createElement('div');
            var newDescription = document.createElement('div');
            var newExample = document.createElement('div');

            newTitle.className = "title";
            newTitle.innerHTML = item.title;

            newDescription.className = " description";
            newDescription.innerHTML = item.description;

            newExample.className = "example";
            newExample.innerHTML = item.example;

            elem.replaceChild(newTitle, elem.childNodes[0]);
            elem.replaceChild(newDescription, elem.childNodes[1]);
            elem.replaceChild(newExample, elem.childNodes[2]);

            counter = counter <= data.data.length - 2 ? counter + 1 : 0;
        });


    }

    function know() {
        ShowDataOnCard();
    }

    var leftButtonEl = document.getElementById("cardButtonLeft");
    var rightButtonEl = document.getElementById("cardButtonRight");

    leftButtonEl.addEventListener("click", function (e) {
        know();
    });

    rightButtonEl.addEventListener("click", function (e) {
        know();
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
    filBlank();
    httpGet('https://google.com');
}