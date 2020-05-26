
class GlobalScopes {
    constructor() {
        this.eventListenerState = {
            menuLinks: { state: false },
            logoutButton: { state: false },
            cardLeftButton: { state: false },
            cardRightButton: { state: false },
            cardLeftButtonMobileVersion: { state: false },
            cardRightButtonMobileVersion: { state: false },
            cardChangeModeStudyButton: { state: false },
            cardChangeModeReviewButton: { state: false },
            studyButtonUnknow: { state: false },
            studyButtonKnow: { state: false },
            studyButtonNextItem: { state: false },
            
            itemListFirstButton: { state: false },
            itemListPrevButton: { state: false },
            itemListNextButton: { state: false },
            itemListLastButton: { state: false },
            itemListButtonFromListToCard: { state: false },
            itemListPutButton: { state: false },
            itemListDeleteButton: { state: false },
            itemListAddButton: { state: false },

            FilmItemListFirstButton: { state: false },
            FilmItemListPrevButton: { state: false },
            FilmItemListNextButton: { state: false },
            FilmItemListLastButton: { state: false },
            FilmItemListButtonFromListToCard: { state: false },
            FilmItemListPutButton: { state: false },
            FilmItemListDeleteButton: { state: false },
            FilmItemListAddButton: { state: false },

            formPutButton: { state: false },
            formAddButton: { state: false }
        }// used in addBubleEventListener() for add once event listener caller

        this.wordOrder = {
            length: 0,
            isFromWordList: false
        }; // used in pageHandlers['word-list'] for eventListener
    }; 

    getEventListenerState() {
        return this.eventListenerState;
    }

    getWordOrder() {
        return this.wordOrder;
    }

}

var globalScopes = new GlobalScopes();

export default globalScopes;