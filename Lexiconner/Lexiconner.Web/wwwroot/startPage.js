
document.addEventListener("DOMContentLoaded", function (event) {
    start();
});

function start() {



    addMenu( testData(20) );

    function addMenu(arrMenuItems) {
        var mainBlock = document.createElement('div');
        mainBlock.className = "main-block-menu";
        mainBlock.id = "mainBlock";

        document.body.appendChild(mainBlock);

        var menu = document.createElement('div');
        menu.className = " menu";
        menu.id = "menuBlock";

        mainBlock.appendChild(menu);


        for (var i = 0; i < arrMenuItems.length; i++) {//

            var menuItems = document.createElement('div');
            menuItems.className = "menu-item";
            menuItems.id = arrMenuItems[i].id;
            menuItems.innerHTML = arrMenuItems[i].name;

            menu.appendChild(menuItems);
        }
    }

    function testData(size) {
        var arrMenuItems = [];

        arrMenuItems.push({
            name: "Show word list",
            id: "ShowWordList"
        });
        arrMenuItems.push({
            name: "Learning words",
            id: "LearningWords"
        });
        arrMenuItems.push({
            name: "Create new words",
            id: "CreateNewWords"
        });

        for (var i = 0; i < size; i++) {
            arrMenuItems.push({});
        }
        return arrMenuItems
    }

}