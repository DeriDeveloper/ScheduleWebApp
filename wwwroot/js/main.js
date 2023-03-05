var UrlThanksDonat = `${window.location.origin}/ThanksDonat`;

var LastIndexNotifyMessage = 0;


window.onload = function () {
    ShowMainBodyLayoutContainer();

    

    document.body.classList.add('loaded_hiding');
    window.setTimeout(function () {
        document.body.classList.add('loaded');
        document.body.classList.remove('loaded_hiding');

        var elementPreloaderSplashScreen = document.getElementById("preloader-splash-screen");
        if (elementPreloaderSplashScreen != null) {
            elementPreloaderSplashScreen.remove();
        }
    }, 500);

    try {
        UpdateCellPreview();

    } catch {

    }


    var buttonShowFeedbackPage = document.getElementById("button-show-feedback-page");
    if (buttonShowFeedbackPage != null) {
        buttonShowFeedbackPage.href = "/Feedback?redirect_url=" + window.location.href;
    }

    var searchNameSelectSchedule = document.getElementById("search-name-select-schedule");

    if (searchNameSelectSchedule != null) {
        searchNameSelectSchedule.addEventListener('input', SearchNameSelectScheduleInput);
       
    }
  



   
    try {
        // отключение zoom через скролл (в том числе трекападами в macOS)
        document.addEventListener('mousewheel', function (e) {
            if (!e.ctrlKey && !e.metaKey) return;

            e.preventDefault();
            e.stopImmediatePropagation();
        }, { passive: false });

        // отключение zoom прикосновениями (в том числе трекападами и т.п.) в Safari и iOS
        document.addEventListener('gesturestart', function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
        }, { passive: false });

        // отключение zoom через клавиатуру (ctrl + "+", ctrl + "-")
        // кнопки браузера для управления zoom отключены не будут
        document.addEventListener('keydown', function (e) {
            if (!e.ctrlKey && !e.metaKey) return;
            if (e.keyCode != 189 && e.keyCode != 187) return;

            e.preventDefault();
            e.stopImmediatePropagation();
        }, { passive: false });
    }
    catch {

    }
}



function CustomClosePopupById(id)
{
    try {
        var popupElement = document.getElementById(`${id}`);

        if (popupElement != null) {
            popupElement.remove();
        }
    } catch (error)
    {

    }
}

function OnClickSaveNote() {
    var noteId = document.getElementById("noteIdInput").value;
    var userId = document.getElementById("userIdInput").value;
    var title = document.getElementById("noteTitleInput").value;
    var description = document.getElementById("noteDescriptionInput").value;

    if (noteId != null && noteId != undefined && userId != null && userId != undefined && title != null && title != undefined && description != null && description != undefined) {
        $.ajax({
            url: '/api/note/save',
            method: 'post',
            dataType: 'json',
            data: JSON.stringify({ note_id: Number(noteId), user_id: Number(userId), title: title, description: description }),
            contentType: "application/json",
            success: function (data, textStatus, jqXHR) {
                AddNotifyMessage("Успешно");
            },
            failure: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            },
            error: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            }
        });
    }
    else {
        AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
    }
}

function OnClickCreateNote() {
    var userId = document.getElementById("userIdInput").value;
    var groupId = document.getElementById("groupIdInput").value;

    if (groupId != null && groupId != undefined && userId != null && userId != undefined) {
        $.ajax({
            url: '/api/note/create',
            method: 'post',
            dataType: 'json',
            data: JSON.stringify({ user_id: Number(userId), group_id: Number(groupId) }),
            contentType: "application/json",
            success: function (data, textStatus, jqXHR) {
                AddNotifyMessage("Успешно");
                var noteId = data.Data.Id;
                document.location = `/Note?id=${noteId}`;

            },
            failure: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            },
            error: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            }
        });
    }
}

function OnClickDeleteNote() {
    var noteId = document.getElementById("noteIdInput").value;
    var userId = document.getElementById("userIdInput").value;

    if (noteId != null && noteId != undefined && userId != null && userId != undefined) {
        $.ajax({
            url: '/api/note/delete',
            method: 'post',
            dataType: 'json',
            data: JSON.stringify({ note_id: Number(noteId), user_id: Number(userId) }),
            contentType: "application/json",
            success: function (data, textStatus, jqXHR) {
                AddNotifyMessage("Успешно");
            },
            failure: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            },
            error: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            }
        });
    }
    else {
        AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
    }
}

function ShowMainBodyLayoutContainer() {
    var mainBodyLayoutContainer = document.getElementById("main-body-layout-container");
    mainBodyLayoutContainer.style = "display:block;";
}

function FeedbackAnswered(id) {
    $.ajax({
        url: '/api/feedback/answered',
        method: 'post',
        dataType: 'json',
        data: JSON.stringify({ id: id }),
        contentType: "application/json",
        success: function (data, textStatus, jqXHR) {
            AddNotifyMessage("Успешно");
            var feedbackContainer = document.getElementById("feedback-container-" + id);
            var feedbackNotAnsweredCount = document.getElementById("feedback-not-answered-count");

            if (feedbackContainer != null) {
                feedbackContainer.remove();
            }

            if (feedbackNotAnsweredCount != null) {
                feedbackNotAnsweredCount.innerText = (Number(feedbackNotAnsweredCount.innerText) - 1).toString();
            }
        },
        failure: function (response) {
            AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
        },
        error: function (response) {
            AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
        }
    });
}


function OnClickSendFeedback(redirectUrl) {
    var feedbackUrlUserInput = document.getElementById("feedback_url_user_input");
    var userNameInput = document.getElementById("user_name_input");
    var messageInput = document.getElementById("message_input");



    if (messageInput != null && userNameInput != null) {
        var userName = userNameInput.value; 
        var feedbackUrlUser = feedbackUrlUserInput.value; 
        var message = messageInput.value; 

        

        if (userName == "") {
            userNameInput.classList.add("input-field-error");

            return;
        }
        else {
            userNameInput.classList.remove("input-field-error");
        }

        

        if (message == "") {
            messageInput.classList.add("input-field-error");

            return;
        }
        else {
            messageInput.classList.remove("input-field-error");
        }


        SendFeedback(redirectUrl, userName, message, feedbackUrlUser);

    }
}

function SendFeedback(redirectUrl, name, message, urlFeedback) {
    $.ajax({
        url: '/api/feedback',
        method: 'post',
        dataType: 'json',
        data: JSON.stringify({ name: name, message: message, urlFeedback: urlFeedback }),
        contentType: "application/json",
        success: function (data, textStatus, jqXHR) {
            AddNotifyMessage("Сообщение успешно отправлено");
            setTimeout(() => {
                Redirect(redirectUrl);
            }, 2000);
            
        },
        failure: function (response) {
            AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
        },
        error: function (response) {
            AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
        }
    });
}

function Redirect(url) {
    if (url != undefined || url != null) {
        window.location.href = url;
    }
}

function ProfileChangeName() {
    var nameElement = document.getElementById("profile-name-input");

    if (nameElement.value != null && nameElement.value != "") {

        var name = nameElement.value;

        var elementProfileNotifyMessages = document.getElementById("profile-notify-messages-container");


        $.ajax({
            url: '/api/UpdateNameUser',
            method: 'post',
            dataType: 'json',
            data: JSON.stringify({ name: name }),
            contentType: "application/json",
            success: function (data, textStatus, jqXHR) {
                AddNotifyMessage("Имя успешно изменено");
            },
            failure: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            },
            error: function (response) {
                AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
            }
        });
    }
}

function SendDonatYoumoney() {

    var methodPay = "AC";
    var amount = document.getElementById("pay-form-amount").value;

    var userId = 0;
    var userIdInput = document.getElementById("pay-form-user-id");
    if (userIdInput.value != null && userIdInput.value != undefined) {
        userId = userIdInput.value;
    }

    var linkPay = `https://yoomoney.ru/quickpay/confirm.xml?receiver=410016336102823&quickpay-form=shop&paymentType=${methodPay}&sum=${amount}&targets=Донат на сайте Расписание&formcomment=Донат на сайте Расписание&short-dest=Донат на сайте Расписание&successURL=${UrlThanksDonat}&label=DonatSiteSchedule;UserId:${userId}\"`;

    window.open(linkPay);
}

function ProfileChangeGroupName() {
    var elementInputGroupName = document.getElementById("profile-select-name-group");

    if (elementInputGroupName != null) {
        if (elementInputGroupName.value != null && elementInputGroupName.value > 0) {
            var elementProfileNotifyMessages = document.getElementById("profile-notify-messages-container");

            $.ajax({
                url: '/api/UpdateGroupIdFromUser',
                method: 'post',
                dataType: 'json',
                data: JSON.stringify({ GroupId: Number(elementInputGroupName.value) }),
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {
                    AddNotifyMessage("Группа успешно изменена");
                },
                failure: function (response) {
                    AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
                },
                error: function (response) {
                    AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
                }
            });
        }
    }
}

function ProfileChangeTheme() {
    var elementInputCheckboxTheme = document.getElementById("profile-select-checkbox-theme");

    if (elementInputCheckboxTheme != null) {
        
            var elementProfileNotifyMessages = document.getElementById("profile-notify-messages-container");

            $.ajax({
                url: '/api/UpdateThemeApp',
                method: 'post',
                dataType: 'json',
                data: JSON.stringify({ theme_id: Number(elementInputCheckboxTheme.checked) }),
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {

                    try {
                        elementInputCheckboxTheme.checked = Boolean(data.Data.Theme);
                        UpdateNameThemeAppUserProfile();
                        //UpdateTheme();
                    }
                    catch  {

                    }

                    AddNotifyMessage("Тема успешно изменёна");
                },
                failure: function (response) {
                    AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
                },
                error: function (response) {
                    AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
                }
            });
        
    }
}

function UpdateTheme() {
    try {
        var themeIdString = GetCookie("data-theme");


        if (themeIdString != "") {

            var themeId = Number(themeIdString);

            if (themeId == 1) {
                var arrayThemeClasses = $('[class^="-theme-light"]');
                var s = 1;
            }
            else {
                var arrayThemeClasses = $('[class^="-theme-dark"]');
                var s = 1;
            }
        }
    } catch {

    }
}



function GetCookie(name) {

    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(name + "=");
        if (c_start != -1) {
            c_start = c_start + name.length + 1;
            c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) {
                c_end = document.cookie.length;
            }
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";

}


function UpdateNameThemeAppUserProfile() {
    var elementInputCheckboxTheme = document.getElementById("profile-select-checkbox-theme");
    var elementLabelProfileTheme = document.getElementById("profile-theme-label");

    if (elementInputCheckboxTheme.checked == true) {
        elementLabelProfileTheme.innerText = "Тёмная";
    }
    else {
        elementLabelProfileTheme.innerText = "Светлая";
    }
}

function ProfileChangeTeacherName() {
    var elementInputTeacherName = document.getElementById("profile-select-name-teacher");

    if (elementInputTeacherName != null) {
        if (elementInputTeacherName.value != null && elementInputTeacherName.value > 0) {
            var elementProfileNotifyMessages = document.getElementById("profile-notify-messages-container");

            $.ajax({
                url: '/api/UpdateTeacherIdFromUser',
                method: 'post',
                dataType: 'json',
                data: JSON.stringify({ TeacherId: Number(elementInputTeacherName.value) }),
                contentType: "application/json",
                success: function (data, textStatus, jqXHR) {
                    AddNotifyMessage("Преподаватель успешно изменён");
                },
                failure: function (response) {
                    AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
                },
                error: function (response) {
                    AddNotifyMessage("Произошла ошибка, попробуйте еще раз");
                }
            });
        }
    }
}

function UpdateProfileSpecial() {

    var elementInputGroupName = document.getElementById("profile-select-name-group");

    if (elementInputGroupName != null) {
        if (elementInputGroupName.value != null && elementInputGroupName.value > 0) {
            var elementProfileSpecialization = document.getElementById("profile-student-specialization");

            if (elementProfileSpecialization != null) {
                $.ajax({
                    url: `/api/GroupInfo?group_id=${elementInputGroupName.value}`,
                    method: 'get',
                    dataType: 'json',
                    success: function (data, textStatus, jqXHR) {
                        if (data["Status"] == "OK") {
                            elementProfileSpecialization.innerText = data["Data"]["Specialization"];
                        }
                        else {
                            elementProfileSpecialization.innerText = "Произошла ошибка, попробуйте еще раз";
                        }
                    },
                    failure: function (response) {
                        elementProfileSpecialization.innerText = "Нет данных";
                    },
                    error: function (response) {
                        elementProfileSpecialization.innerText = "Нет данных";
                    }
                });
            }
        }
    }
}

function SaveProfile() {
    var elementProfileNotifyMessages = document.getElementById("profile-notify-messages-container");

    
    //ClearNotifyMessages(elementProfileNotifyMessages);
    

    ProfileChangeName();
    ProfileChangeGroupName();
    ProfileChangeTeacherName();
    ProfileChangeTheme();
    UpdateProfileSpecial();

}

function GetElementContainerNotifyMessages() {
    return document.getElementById("container-notify-messages");
}

function CreateNotifyMessageContainer(text) {
    LastIndexNotifyMessage = LastIndexNotifyMessage + 1;

    var currentIndexNotifyMessage = LastIndexNotifyMessage + 1;

    var elementParagraphTextMessage = document.createElement("p");
    elementParagraphTextMessage.style.wordBreak = "break-word";
    elementParagraphTextMessage.textContent = text;

    var elementParagraphCloseButtonMessage = document.createElement("p");
    elementParagraphCloseButtonMessage.style.cursor = "pointer";
    elementParagraphCloseButtonMessage.style.marginLeft = "10px";
    elementParagraphCloseButtonMessage.textContent = "x";
    elementParagraphCloseButtonMessage.addEventListener('click', function () { CloseNotifyMessage(currentIndexNotifyMessage); });

    var elementDivNotifyMessageContainerItem = document.createElement("div");
    elementDivNotifyMessageContainerItem.id = `NotifyMessage${currentIndexNotifyMessage}`;
    elementDivNotifyMessageContainerItem.classList.add("notify-message-container-item");
    elementDivNotifyMessageContainerItem.classList.add("background-notify-message");
    elementDivNotifyMessageContainerItem.classList.add("border-color-notify-message");

    elementDivNotifyMessageContainerItem.appendChild(elementParagraphCloseButtonMessage);
    elementDivNotifyMessageContainerItem.appendChild(elementParagraphTextMessage);

    setTimeout(function () {
        CloseNotifyMessage(currentIndexNotifyMessage);
    }, 10000);

    return elementDivNotifyMessageContainerItem;
}


function AddNotifyMessage(notifyMessageText) {
    var elementContainerNotifyMessages = GetElementContainerNotifyMessages();

    if (elementContainerNotifyMessages != null) {
        elementContainerNotifyMessages.appendChild(CreateNotifyMessageContainer(notifyMessageText));
    }
}

function ClearNotifyMessages() {
    var elementContainerNotifyMessages = GetElementContainerNotifyMessages();


    if (elementContainerNotifyMessages != null) {
        elementContainerNotifyMessages.innerHTML = "";
    }
}

function SearchNameSelectScheduleInput(e) {
    var searchText = e.target.value.toLowerCase();

    var groupsGroupNameSelectScheduleElement = document.getElementById("groups-group-name-select-schedule");
    var groupsGroupNameSelectScheduleChildrenDivs = groupsGroupNameSelectScheduleElement.children;
    //console.dir(groupsGroupNameSelectScheduleChildrenDivs);

    for (var i = 0; i < groupsGroupNameSelectScheduleChildrenDivs.length; i++) {
        var selectGroupNameSelectScheduleDiv = groupsGroupNameSelectScheduleChildrenDivs[i];
        var text = selectGroupNameSelectScheduleDiv.innerText;
        
        if (NormalizeString(text).toLowerCase().includes(searchText)){
            selectGroupNameSelectScheduleDiv.style.display = "block";
        }
        else {
            selectGroupNameSelectScheduleDiv.style.display = "none";

        }
    }


    var groupsTeacherNameSelectScheduleElement = document.getElementById("groups-teacher-name-select-schedule");
    var groupsTeacherNameSelectScheduleChildrenDivs = groupsTeacherNameSelectScheduleElement.children;
    //console.dir(groupsTeacherNameSelectScheduleChildrenDivs);

    for (var i = 0; i < groupsTeacherNameSelectScheduleChildrenDivs.length; i++) {
        var selectTeacherNameSelectScheduleDiv = groupsTeacherNameSelectScheduleChildrenDivs[i];
        var text = selectTeacherNameSelectScheduleDiv.innerText;

        if (NormalizeString(text).toLowerCase().includes(searchText)) {
            selectTeacherNameSelectScheduleDiv.style.display = "block";
        }
        else {
            selectTeacherNameSelectScheduleDiv.style.display = "none";

        }
    }
}





var ArrayNormalizeStrings = [
    "\n","\r", "\t"
]

function NormalizeString(string) {
    for (var i = 0; i < ArrayNormalizeStrings.length; i++) {
        string = string.replace(ArrayNormalizeStrings[i], "");
    }

    return string;
}

function CloseNotifyMessage(id) {
    var notifyMessage = document.getElementById("NotifyMessage" + id);
    if (notifyMessage != null) {
        notifyMessage.remove();
    }
}

Element.prototype.remove = function () {
    this.parentElement.removeChild(this);
}
NodeList.prototype.remove = HTMLCollection.prototype.remove = function () {
    for (var i = this.length - 1; i >= 0; i--) {
        if (this[i] && this[i].parentElement) {
            this[i].parentElement.removeChild(this[i]);
        }
    }
}

function AuthenticationStudentRegistrationShowBtn(index) {
    var AuthenticationStudentViewEnter = document.getElementById("authentication-form-student-enter");
    var AuthenticationStudentViewRegistration = document.getElementById("authentication-form-student-registration");

    if (index == 0) {
        AuthenticationStudentViewRegistration.style.cssText = "display: block;";
        AuthenticationStudentViewEnter.style.cssText = "display: none;";
    }
    else if (index == 1) {
        AuthenticationStudentViewEnter.style.cssText = "display: block;";
        AuthenticationStudentViewRegistration.style.cssText = "display: none;";
    }
}

function AuthenticationTeacherRegistrationShowBtn(index) {
    var AuthenticationTeacherViewEnter = document.getElementById("authentication-form-teacher-enter");
    var AuthenticationTeacherViewRegistration = document.getElementById("authentication-form-teacher-registration");

    if (index == 0) {
        AuthenticationTeacherViewRegistration.style.cssText = "display: block;";
        AuthenticationTeacherViewEnter.style.cssText = "display: none;";
    }
    else if (index == 1) {
        AuthenticationTeacherViewEnter.style.cssText = "display: block;";
        AuthenticationTeacherViewRegistration.style.cssText = "display: none;";
    }
}


function AuthenticationTypePersonBtn(index) {
    var AuthenticationFormStudent = document.getElementById("authentication-form-student");
    var AuthenticationFormTeacher = document.getElementById("authentication-form-teacher");
    var AuthenticationTypePersonStudentBtn = document.getElementById("authentication-type-person-student-btn");
    var AuthenticationTypePersonTeacherBtn = document.getElementById("authentication-type-person-teacher-btn");

    if (index == 0) {
        AuthenticationFormTeacher.style.display = "none";
        AuthenticationFormStudent.style.display = "block";
        AuthenticationTypePersonStudentBtn.style.backgroundColor = "#9457EB";
        AuthenticationTypePersonTeacherBtn.style.backgroundColor = "";
        AuthenticationTypePersonStudentBtn.style.color = "#fff";
        AuthenticationTypePersonTeacherBtn.style.color = "#000";
    }
    else if (index == 1) {
        AuthenticationFormTeacher.style.display = "block";
        AuthenticationFormStudent.style.display = "none";
        AuthenticationTypePersonStudentBtn.style.backgroundColor = "";
        AuthenticationTypePersonTeacherBtn.style.backgroundColor = "#9457EB";
        AuthenticationTypePersonTeacherBtn.style.color = "#fff";
        AuthenticationTypePersonStudentBtn.style.color = "#000";
    }
}


function UpdateCellPreview() {
    var selectNumberPair = document.getElementById("NumberPair");
    var numberPair = selectNumberPair.options[selectNumberPair.selectedIndex].text;
    document.getElementById("cell-number-pair").innerHTML = numberPair;


    EditChangeNamesPair(document.getElementById("NamesPair"));
    EditChangeTeachersPair(document.getElementById("TeachersPair"));
    EditChangeAudiencesPair(document.getElementById("AudiencesPair"));
    EditChangeTimeStart(document.getElementById("TimeStart"));
    EditChangeTimeEnd(document.getElementById("TimeEnd"));
    UpdateChangeChekbox(document.getElementById("ChangeCell"));

}

function UpdateChangeChekbox(input) {

    var ElementCellContainer = document.getElementById("cell-container");

    var ElementChangeCell = document.getElementById("ChangeCell");
    var ElementChangeCellHidden = document.getElementById("ChangeCellHidden");
    var changeCell = ElementChangeCell.checked;
    if (changeCell) {
        ElementChangeCellHidden.value = true;
        ElementCellContainer.style.backgroundColor = "#A3B6DE";
    }
    else {
        ElementChangeCellHidden.value = false;
        ElementCellContainer.style.backgroundColor = "#C6D3DE";
    }
}


function HideAllScheduleDays() {
    try {
        for (var i = 0; i < 7; i++) {
            try {
                document.getElementById("ScheduleDay_" + i).style.cssText = "display: none;";
            }
            catch (err) {
                //console.log(err);
                //alert("HideAllScheduleDays -> for dayOfWeek -> " + err);
            }
        }
    }
    catch (err)
    {
        console.log(err);
        //alert("HideAllScheduleDays -> " + err);
    }
}

function ClearActiveBtnDayOfWeek() {
    var arrayButton = [];

    arrayButton.push(document.getElementById("day-of-week-button_0"));
    arrayButton.push(document.getElementById("day-of-week-button_1"));
    arrayButton.push(document.getElementById("day-of-week-button_2"));
    arrayButton.push(document.getElementById("day-of-week-button_3"));
    arrayButton.push(document.getElementById("day-of-week-button_4"));
    arrayButton.push(document.getElementById("day-of-week-button_5"));


    arrayButton.forEach(function (item, i, arrayButton) {
        item.classList.remove("day-of-week-button-active");
        item.style.color = "#000";
    });

    var textOne = document.getElementById("day-of-week-text_0");
    var textTwo = document.getElementById("day-of-week-text_1");
    var textThree = document.getElementById("day-of-week-text_2");
    var textFoure = document.getElementById("day-of-week-text_3");
    var textFive = document.getElementById("day-of-week-text_4");
    var textSix = document.getElementById("day-of-week-text_5");
    textOne.classList.remove("day-of-week-text-active");
    textTwo.classList.remove("day-of-week-text-active");
    textThree.classList.remove("day-of-week-text-active");
    textFoure.classList.remove("day-of-week-text-active");
    textFive.classList.remove("day-of-week-text-active");
    textSix.classList.remove("day-of-week-text-active");
}

function ShowScheduleDayByIndex(idBtn, parametetsURL) {
    try
    {
        window.location.replace(location.protocol + location.hostname + "/" + parametetsURL);
    }
    catch (err) {
        console.log(err);
        //alert("ShowScheduleDayByIndex -> " + err);
    }
}


function DeleteCellSchedule() {
    var ElementDayOfWeek = document.getElementById("DayOfWeek");
    var dayOfWeek = ElementDayOfWeek.value;

    var ElementTypeCell = document.getElementById("TypeCell");
    var typeCell = ElementTypeCell.value;

    var ElementNumberPair = document.getElementById("NumberPair");
    var numberPair = ElementNumberPair.value;

    var ElementChangeCell = document.getElementById("ChangeCell");
    var changeCell = ElementChangeCell.checked;

    var url = window.location.href;

    if (dayOfWeek.length > 0) {

        url += "&day_of_week=" + dayOfWeek


        if (typeCell.length > 0) {


            url += "&type_cell=" + typeCell


            if (numberPair.length > 0) {

                url += "&number_pair=" + numberPair

                url += "&change_cell=" + changeCell

                url += "&command=delete";

                //ShowMessage(url);

                window.location.href = url;
            }
            else {
                ElementNumberPair.classList.add("error-select");
            }
        }
        else {
            ElementTypeCell.classList.add("error-select");
        }
    }
    else {
        ElementDayOfWeek.classList.add("error-select");
    }

}

function OnChangeCell() {
    var ElementChangeCell = document.getElementById("ChangeCell");
    var changeCellStatus = ElementChangeCell.checked;
    //ShowMessage(changeCellStatus);
}

function SaveCellSchedule() {
    var ElementDayOfWeek = document.getElementById("DayOfWeek");
    var dayOfWeek = ElementDayOfWeek.value;

    var ElementTypeCell = document.getElementById("TypeCell");
    var typeCell = ElementTypeCell.value;

    var ElementNumberPair = document.getElementById("NumberPair");
    var numberPair = ElementNumberPair.value;

    var ElementTimeStart = document.getElementById("TimeStart");
    var timeStart = ElementTimeStart.value;

    var ElementTimeEnd = document.getElementById("TimeEnd");
    var timeEnd = ElementTimeEnd.value;

    var ElementChangeCell = document.getElementById("ChangeCell");
    var changeCell = ElementChangeCell.checked;


    var ElementNamesPair = document.getElementById("NamesPair");
    var ElementTeachersPair = document.getElementById("TeachersPair");
    var ElementAudiencesPair = document.getElementById("AudiencesPair");



    var namesPair = arrayNamesPair.join(",");
    var teachersPair = arrayTeachersPair.join(",");
    var audiencesPair = arrayAudiencesPair.join(",");




    ElementDayOfWeek.classList.remove("error-select");
    ElementTypeCell.classList.remove("error-select");
    ElementNumberPair.classList.remove("error-select");
    ElementTimeStart.classList.remove("error-select");
    ElementTimeEnd.classList.remove("error-select");
    ElementNamesPair.classList.remove("error-select");
    ElementTeachersPair.classList.remove("error-select");
    ElementAudiencesPair.classList.remove("error-select");




    var url = window.location.href;

    if (dayOfWeek.length > 0) {

        url += "&day_of_week=" + dayOfWeek


        if (typeCell.length > 0) {


            url += "&type_cell=" + typeCell


            if (numberPair.length > 0) {

                url += "&number_pair=" + numberPair


                if (namesPair.length >= 0) {

                    url += "&names_pair=" + namesPair

                }
                else {
                    ElementNamesPair.classList.add("error-select");
                }


                if (teachersPair.length >= 0) {

                    url += "&teachers_pair=" + teachersPair



                }
                else {
                    ElementTeachersPair.classList.add("error-select");
                }


                if (audiencesPair.length >= 0) {

                    url += "&audiences_pair=" + audiencesPair

                }
                else {
                    ElementAudiencesPair.classList.add("error-select");
                }

                if (timeStart.length > 0 && timeEnd.length > 0) {
                    url += "&time_start=" + timeStart;
                    url += "&time_end=" + timeEnd;
                }


                url += "&change_cell=" + changeCell


                url += "&command=add";

                window.location.href = url;
            }
            else {
                ElementNumberPair.classList.add("error-select");
            }
        }
        else {
            ElementTypeCell.classList.add("error-select");
        }
    }
    else {
        ElementDayOfWeek.classList.add("error-select");
    }
}


function ShowMessage(message) {
    console.log(message);
    alert(message);
}


function EditChangeDayOfWeek(input)
{
    var temp = input.value;
    //ShowMessage(temp);
}


function EditChangeNumberPair(input) {
    var temp = input.value;
    document.getElementById("cell-number-pair").innerHTML = temp;
}


function EditChangeTimeStart(input) {
    var temp = input.value;
    document.getElementById("cell-time-start").innerHTML = temp;
}


function EditChangeTimeEnd(input) {
    var temp = input.value;
    document.getElementById("cell-time-end").innerHTML = temp;
}


function EditChangeNamesPair(select) {
    arrayNamesPair = [];
    var result = [];
    var options = select.options && select;
    var option;

    for (var i = 0; i < options.length; i++) {
        option = options[i];

        if (option.selected) {
            if (Contains(arrayNamesPair, option.text)) {
                const index = arrayNamesPair.indexOf(option.text);
                if (index > -1) {
                    arrayNamesPair.splice(index, 1);
                }
            }
            else {
                result.push(option.text);
                arrayNamesPair.push(option.text);
            }
        }
    }
    document.getElementById("cell-names-pair").innerHTML = arrayNamesPair.join(",<br/>");
}


function EditChangeTeachersPair(select) {
    arrayTeachersPair = [];
    var result = [];
    var options = select.options && select;
    var option;

    for (var i = 0; i < options.length; i++) {
        option = options[i];

        if (option.selected) {
            if (Contains(arrayTeachersPair, option.text)) {
                const index = arrayTeachersPair.indexOf(option.text);
                if (index > -1) {
                    arrayTeachersPair.splice(index, 1);
                }
            }
            else {
                result.push(option.text);
                arrayTeachersPair.push(option.text);
            }
        }
    }
    document.getElementById("cell-teachers-pair").innerHTML = arrayTeachersPair.join(",<br/>");
}


function EditChangeAudiencesPair(select) {
    arrayAudiencesPair = [];
    var result = [];
    var options = select.options && select;
    var option;

    for (var i = 0; i < options.length; i++) {
        option = options[i];

        if (option.selected) {
            if (Contains(arrayAudiencesPair, option.text)) {
                const index = arrayAudiencesPair.indexOf(option.text);
                if (index > -1) {
                    arrayAudiencesPair.splice(index, 1);
                }
            }
            else {
                result.push(option.text);
                arrayAudiencesPair.push(option.text);
            }
        }
    }

    document.getElementById("cell-audiences-pair").innerHTML = arrayAudiencesPair.join(",<br/>");
}


function Contains(array, element) {
    return array.indexOf(element) != -1;
}               

                