
class GlobalScopes {
    constructor() {
        this.eventListenerState = {
            menuLinks: { state: false },
            logoutButton: { state: false },
            cardLeftButton: { state: false },
            cardRightButton: { state: false },
            cardLeftButtonMobileVersion: { state: false },
            cardRightButtonMobileVersion: { state: false },
            itemListFirstButton: { state: false },
            itemListPrevButton: { state: false },
            itemListNextButton: { state: false },
            itemListLastButton: { state: false },
            itemListButtonFromListToCard: { state: false },
            itemListPutButton: { state: false },
            itemListDeleteButton: { state: false },
            itemListAddButton: { state: false },
            formPutButton: { state: false },
            formAddButton: { state: false }
        }

        this.wordOrder = {
            length: 0,
            isFromWordList: false
        }; // used in pageHandlers['word-list'] for eventListener
    }; // used for addBubleEventListener() for add one event listener

    getEventListenerState(){
        return this.eventListenerState;
    }

    getWordOrder(){
        return this.wordOrder;
    }

}

var globalScopes = new GlobalScopes();

export default globalScopes;