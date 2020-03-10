import Vue from 'vue';
import notificationUtil from '@/utils/notification';

class UserConfirmationUtil {
    constructor() {

    }

    /**
     * Handles calls to API endpoints that require user confirmations.
     * actionF: API call function actionF({userConfirmation}) -> returns Promise<Object userConfirmationDataResponse>, 
     * where userConfirmationDataResponse is {data, userConfirmation: {isProcessed, confirmations, processedConfirmations, notifications}}
     */
    handleUserConfirmationFlow({actionF}) {
        return this._handleUserConfirmationFlow({actionF, userConfirmationRequest: null});
    }

    _handleUserConfirmationFlow({
        actionF, 
        userConfirmationRequest = null, // request data
    }) {
        let initialConfirm = userConfirmationRequest ? true : window.confirm('Are you sure?');
        if(initialConfirm) {
            // call action (server API)
            return actionF({userConfirmation: userConfirmationRequest}).then((responseData) => {
                let userConfirmationDataResponse = responseData;
                let {data, userConfirmation} = userConfirmationDataResponse;
                let {isProcessed, confirmations, processedConfirmations, notifications} = userConfirmation || {};

                if(confirmations && confirmations.length !== 0) {
                    // ask about confirmation (all)
                    let confirmationResponses = confirmations.map(confirmation => {
                        let {confirmationId, confirmationText} = confirmation;
                        let isConfirmed = window.confirm(confirmationText);
                        return {
                            confirmationId,
                            isConfirmed,
                        };
                    });

                    // retry request
                    let userConfirmationRequest = {
                        confirmations: [
                            ...processedConfirmations,
                            ...confirmationResponses,
                        ],
                    };
                    return {userConfirmationRequest, isProcessed: isProcessed}
                } else {
                    // no confirmations required. action was confirmed and completed or not confirmed and not completed
                    if(isProcessed) {
                        // processed
                        this._showNotifications(notifications, 'success');
                    } else {
                        // not processed
                        this._showNotifications(notifications, 'information');
                    }

                    return {userConfirmationRequest: null, isProcessed: isProcessed}
                }
            }).then(({userConfirmationRequest = null, isProcessed = false}) => {
                // handle recursion here
                if(userConfirmationRequest) {
                    // make recursive call
                    return this._handleUserConfirmationFlow({actionF, userConfirmationRequest});
                } else {
                    // return result (recursion chain resolved)
                    return {isProcessed};
                }
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

        return Promise.resolve({isProcessed: false});
    }

    _showNotifications(notifications, type) {
        if(notifications && notifications.length !== 0 && type) {
            notifications.map(({title, text}) => {
                Vue.notify({
                    group: 'app-important',
                    type: type,
                    title: title,
                    text: text,
                });
            });
        }
    }


    // Example of handling in component manually
    // deleteAccount: function(userConfirmation = null) {
        //     let initialConfirm = userConfirmation ? true : window.confirm('Are you sure?');
        //     if(initialConfirm) {
        //         this.$store.dispatch(storeTypes.AUTH_USER_ACCOUNT_DELETE, {userConfirmation}).then((responseData) => {
        //             let userConfirmationDataResponse = responseData;
        //             let {data, userConfirmation} = userConfirmationDataResponse;
        //             let {isProcessed, confirmations, processedConfirmations, notifications} = userConfirmation || {};

        //             // TODO: maybe would be better to move confirmation logic to some helper to avoid duplicated code

        //             const showNotifications = (notifications, type) => {
        //                 if(notifications && notifications.length !== 0 && type) {
        //                     notifications.map(({title, text}) => {
        //                         this.$notify({
        //                             group: 'app-important',
        //                             type: type,
        //                             title: title,
        //                             text: text,
        //                         });
        //                     });
        //                 }
        //             };

        //             if(confirmations && confirmations.length !== 0) {
        //                 // ask about confirmation (all)
        //                 let confirmationResponses = confirmations.map(confirmation => {
        //                     let {confirmationId, confirmationText} = confirmation;
        //                     let isConfirmed = window.confirm(confirmationText);
        //                     return {
        //                         confirmationId,
        //                         isConfirmed,
        //                     };
        //                 });

        //                 // retry request
        //                 let userConfirmation = {
        //                     confirmations: [
        //                         ...processedConfirmations,
        //                         ...confirmationResponses,
        //                     ],
        //                 };
        //                 this.deleteAccount(userConfirmation);
        //             } else {
        //                 // no confirmations required. action was confirmed and completed or not confirmed and not completed
        //                 if(isProcessed) {
        //                     // deleted
        //                     showNotifications(notifications, 'success');

        //                     authService.logoutWithoutRedirect().then(() => {
        //                         this.$router.push({name: 'home'});
        //                     }).catch(err => {
        //                         console.error(err);
        //                         window.alert('Something went wrong. Try to refresh the page.');
        //                     });
        //                 } else {
        //                     // not deleted
        //                     showNotifications(notifications, 'information');
        //                 }
        //             }
        //         }).catch(err => {
        //             console.error(err);
        //             notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        //         });
        //     }
        // },
}   

export default new UserConfirmationUtil();
