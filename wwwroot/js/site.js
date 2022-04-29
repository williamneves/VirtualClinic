// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})


function loginModal(partial) {
    var modalExist = document.getElementById("exampleModal");
    var myModal;
    if (!modalExist) {
        axios
            .get(partial)
            .then((res) => {
                //console.log(res.data);
                let htmlElement = createHtml(res.data);
                document.getElementById(partial).append(htmlElement);

                myModal = new bootstrap.Modal(document.getElementById("exampleModal"), {
                    keyboard: false,
                });
                myModal.toggle();

                return this;
            })
            .catch((error) => {
                console.log(error);
            });
    }
    myModal = new bootstrap.Modal(document.getElementById("exampleModal"), {
        keyboard: false,
    });
    myModal.toggle();
}
function createHtml(data) {
    let htmlElement = document.createElement("div");
    htmlElement.innerHTML = data;
    return htmlElement;
}

// Call Medical Notes

function getMedicalNotes(id) {
    return new Promise((resolve, reject) => {
        $("#accordionMedicalNotes").fadeIn('slow');
        document.getElementById("medical-notes-card").style.opacity = "1";

        $.when($.ajax(
            {
                url: `json/medicalnotes/${id}`,
                method: 'GET'
            })).done(function (data) {

                $('#First-Accordion-aHPI-ajax').html(data.hpi);
                $('#First-Accordion-aPE-ajax').html(data.pe);
                $('#First-Accordion-aSMRY-ajax').html(data.summary);
                $('#First-Accordion-aAP-ajax').html(data.ap);

                let apptid = data.appointmentId
                // Get appointment info
                $.when($.ajax(
                    {
                        url: `json/appointments/${apptid}`,
                        method: 'GET'
                    })).done(function (apptData) {
                        $('#apptId-card-mn-ajax').html(apptData.appointmentId);
                        let date = new Date(apptData.dateTime);
                        $('#apptDate-card-mn-ajax').html(date.toDateString() + " at " + date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true }));
                    });



                resolve();

            }).fail(function (error) {
                reject(error);
            });

    });
}

// Auto Save Medical Notes
// Set Interval function variable
let auto_refresh;

// Catch the form submit forms by .text-area-notes
document.querySelectorAll('.text-area-notes').forEach(function (element) {
    // Add event listener Focus to each element, to catch the focus event and start the auto save
    element.addEventListener('focus', function (evt) {
        autosaveForm(true);
    });

    // Add event listener Blur to each element, to catch the blur event and stop the auto save
    element.addEventListener('blur', function (evt) {
        autosaveForm(false);
    });
});



// Auto Save Function 
// Set parameter run to true or false, set parameter FormID to the ID of the form, set the parameter interval to the interval of the auto save
function autosaveForm(run = false, FormID = "form-medical-note-pd", interval = 500) {

    // If run is true, start the auto save
    if (run) {
        auto_refresh = setInterval(
            function () {
                post();
            }, interval);
    }
    // If run is false, stop the auto save
    else {
        clearInterval(auto_refresh);
    }

    // Function to Save the form (if is new, backend will create a new record, if is update, backend will update the record)
    function post() {

        let form = document.getElementById(FormID);
        let formAction = form.getAttribute("action");
        let formMethod = form.getAttribute("method");

        // Using Axios js library to make the post request
        axios({
            method: formMethod,
            url: formAction,
            data: $(form).serialize(),
            dataType: 'json',
        })
            .then((res) => {
                console.log(res);
            })
            .catch((err) => {
                console.log(err);
            });
    }

}



// const tx = document.querySelectorAll('.text-area-notes');
// // const tx = document.getElementsByTagName("textarea");
// for (let i = 0; i < tx.length; i++) {
//     tx[i].setAttribute("style", "height:" + (tx[i].scrollHeight) + "px;overflow-y:hidden;");
//     tx[i].addEventListener("input", OnInput, false);
//     tx[i].addEventListener("change", OnInput, false);
// }
//
// function OnInput() {
//     this.style.height = "auto";
//     this.style.height = (this.scrollHeight) + "px";
// }

$('.text-area-notes').each(function () {
    autosize(this);
}).on('autosize:resized', function () {
    console.log('textarea height updated');
});


// Auto Save Medical Notes - End

// // autosaveForm();
//
// // Add event listener for all selector
//
//
//
// MedicalNoteForm.addEventListener("submit", function (event) {
//   event.preventDefault();
//   let form = event.target;
//   let formData = new FormData(form);
//   let formAction = form.getAttribute("action");
//   let formMethod = form.getAttribute("method");
//   let formDataObj = {};
//   formData.forEach((value, key) => {
//     formDataObj[key] = value;
//     console.log("Update");
//   });
//   axios({
//     method: formMethod,
//     url: formAction,
//     data: $(MedicalNoteForm).serialize(), 
//       dataType: 'json',
//   })
//     .then((res) => {
//       console.log(res);
//       if (res.data.success) {
//         // Success
//         let successMessage = res.data.message;
//         let successClass = "alert-success";
//         let successId = "success-message";
//         let successElement = createAlert(
//           successMessage,
//           successClass,
//           successId
//         );
//         form.insertAdjacentElement("afterend", successElement);
//         setTimeout(() => {
//           successElement.remove();
//         }, 3000);
//       } else {
//         // Error
//         let errorMessage = res.data.message;
//         let errorClass = "alert-danger";
//         let errorId = "error-message";
//         let errorElement = createAlert(
//           errorMessage,
//           errorClass,
//           errorId
//         );
//         form.insertAdjacentElement("afterend", errorElement);
//         setTimeout(() => {
//           errorElement.remove();
//         }, 3000);
//       }
//     })
//     .catch((error) => {
//       console.log(error);
//     });
// });
// // function submitmedicalnote(event){
// //     console.log(event.target.HPI);
// //     console.log(event.target.PE);
// //     console.log(event.target.Summary);
// // 

// Function to reload the page
function reloadPage() {
    
    // setTimeout with 3 seconds
    setTimeout(function () {
        // reload the page
        location.reload();
    }, 3500);
}

// Get date like ddd dd, MMM, yyyy @ hh:mm tt with javascript
function getDateTime() {
// get a new date (locale machine date time)
    var date = new Date();
// get the date as a string
    var n = date.toDateString();
// get the time as a string
    var time = date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: true });
    
    let topmenu = document.getElementById("timetopmenu");
    topmenu.innerHTML = ' @ ' + time;
}
function refreshEveryInterval(interval){
    // setInterval with 60 seconds
    console.log("Refresh every " + interval + " seconds");
    getDateTime();
    
    setInterval(function () {
        // reload the page
        console.log("Refreshed");
        getDateTime();
    }, interval);
}
refreshEveryInterval(60000);



// Copy to clipboard
function autoCopyURL(classname) {
    /* Get the text field */
    var copyText = document.querySelector(`.${classname}`);

    /* Select the text field */
    copyText.select();
    copyText.setSelectionRange(0, 99999); /* For mobile devices */

    /* Copy the text inside the text field */
    navigator.clipboard.writeText(copyText.value);

    /* Alert the copied text */
    alert("Copied!\n" + copyText.value);
}


// Create Room Video API
function createRoom(element = null,apptId){
    if(element){
        element.classList.add('disabled');
    }
    
    let videoUrl = "";
    
    axios({
        method: 'GET',
        url: '/videoapi/createroom/',
    })
        .then((res) => {
            console.log(res, res.data["videoUrl"]);
            videoUrl = res.data["videoUrl"];
            let dataSaveRoom = `{
                "videoUrl": "${res.data["videoUrl"]}",
                "videoRoom": "${res.data["videoRoom"]}",
                "apptId": "${apptId}"
            }`;
            console.log(dataSaveRoom);
            console.log("sending data to save room");
            saveRoom(apptId,res.data["videoRoom"]);
            
        })
        .catch((error) => {
            console.log(error);
        });

    // element.classList.remove('disabled');
    return videoUrl;
    
}
// Save Room at Appointment
function saveRoom(apptId,room){
    console.log(room);
    console.log(apptId);
    axios({
        method: 'POST',
        url: `/provider/appt/setvideourl/${room}/${apptId}`,
        })
        .then((res) => {
            console.log(res);
            if (res.data.success) {
                console.log(res);
            } else {
                console.log(res);
            }
        })
        .catch((error) => {
            console.log(error);
        });
}

function getMedicalNotes(id){
    return new Promise((resolve, reject) => {
        $("#accordionMedicalNotes").fadeIn('slow');
        document.getElementById("medical-notes-card").style.opacity="1";

        $.when($.ajax(
            {
                url: `json/medicalnotes/${id}`,
                method: 'GET'
            })).done(function(data){

            $('#First-Accordion-aHPI-ajax').html(data.hpi);
            $('#First-Accordion-aPE-ajax').html(data.pe);
            $('#First-Accordion-aSMRY-ajax').html(data.summary);
            $('#First-Accordion-aAP-ajax').html(data.ap);

            let apptid = data.appointmentId
            // Get appointment info
            $.when($.ajax(
                {
                    url: `json/appointments/${apptid}`,
                    method: 'GET'
                })).done(function(apptData){
                $('#apptId-card-mn-ajax').html(apptData.appointmentId);
                let date = new Date(apptData.dateTime);
                $('#apptDate-card-mn-ajax').html(date.toDateString() +" at "+ date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true }));
            });



            resolve();

        }).fail(function(error){
            reject(error);
        });

    });
}

// Attendence Page
function startAttendence(element,apptId,videoUrl){
    element.classList.add('disabled');
    element.classList.remove('btn-primary');
    element.classList.add('btn-secondary');
    document.querySelector("#addmedicationbtn").disabled = false;
    document.querySelector("#btn-finish-left").disabled = false;
    
    // Select all textarea elements
    let textareas = document.querySelectorAll('textarea');
    // Loop through the textarea elements
    for (let i = 0; i < textareas.length; i++) {
        // Set the textarea to readonly
        textareas[i].disabled = false;
    }
    
    // make a post request to the server to start the attendence
    axios({
        method: 'POST',
        url: `/provider/appt/status/${apptId}/inprogress/`,
        })
        .then((res) => {
            // console.log(res);
            if (res.data.success) {
                // console.log(res);
            } else {
                // console.log(res);
            }
        })
        .catch((error) => {
            // console.log(error);
        });
    
    console.log(videoUrl);
    
    
    // Set a timer to exec another function after 5 seconds
    setTimeout(function() {
        let urlvideo;
        if(videoUrl == null){
            // Call the frame video
            
            let videourl = createRoom(null,apptId)
            videoCallFrame(videourl)
        }
        else{
            
            // Call the frame video
            videoCallFrame(videoUrl)
        }
    }, 5000);
    
    // Create video room if not exist or expired
    
}
function videoCallFrame(videoUrl){
    
    const MY_IFRAME = document.createElement('iframe');
    MY_IFRAME.setAttribute(
        'allow',
        'microphone; camera; autoplay; display-capture'
    );

    const iframeProperties = { url: `${videoUrl}` };

    let videoIframe = document.getElementById('videoConferenceIframe');
    let videocallblock = videoIframe.appendChild(MY_IFRAME);

    videoIframe.style.height = '470px';
    videocallblock.style.width = '100%';
    videocallblock.style.height = '100%';
    videocallblock.classList = ["iframevideo"];

    let callFrame = DailyIframe.wrap(MY_IFRAME, iframeProperties);

    callFrame.join();
}

// Finish Attendence Page
function finishAttend(element,apptId){
    element.classList.add('disabled');
    element.classList.remove('btn-primary');
    element.classList.add('btn-secondary');
    // document.querySelector("#btn-Finish-visit").disabled = true;
    document.querySelector("#addmedicationbtn").disabled = true;
    // Select all textarea elements
    let textareas = document.querySelectorAll('textarea');
    // Loop through the textarea elements
    for (let i = 0; i < textareas.length; i++) {
        // Set the textarea to readonly
        textareas[i].disabled = true;
    }
    // make a post request to the server to start the attendence
    axios({
        method: 'POST',
        url: `/provider/appt/status/${apptId}/done/`,
        })
        .then((res) => {
            // console.log(res);
            if (res.data.success) {
                // console.log(res);
            } else {
                // console.log(res);
            }
        })
        .catch((error) => {
            // console.log(error);
        });
}


// DataTables
let apptTable1 = new DataTable('#apptTable1', {
    // options
    // scrollY: "300px",
    scrollCollapse: true,
    paging: true,
    select: true,
    "dom": '<"card-header d-flex px-3 pt-2 justify-content-between align-items-center"lf><"bg-light m-0"rt><"card-footer d-flex px-3 pb-2 justify-content-between align-items-center"<"info-pag"i>p><"clear">'
});

let apptTableAllToday = new DataTable('#apptTableAllToday', {
    // options
    // scrollY: "300px",
    scrollCollapse: true,
    paging: true,
    select: true,
    "dom": '<"card-header d-flex px-3 pt-2 justify-content-between align-items-center"lf><"bg-light m-0"rt><"card-footer d-flex px-3 pb-2 justify-content-between align-items-center"<"info-pag"i>p><"clear">'
});

let apptTablePatientWating = new DataTable('#apptTablePatientWating', {
    // options
    // scrollY: "300px",
    scrollCollapse: true,
    paging: true,
    select: true,
    "dom": '<"card-header d-flex px-3 pt-2 justify-content-between align-items-center"lf><"bg-light m-0"rt><"card-footer d-flex px-3 pb-2 justify-content-between align-items-center"<"info-pag"i>p><"clear">'
});
let apptTablePatientToday = new DataTable('#apptTablePatientToday', {
    // options
    // scrollY: "300px",
    scrollCollapse: true,
    paging: true,
    select: true,
    "dom": '<"card-header d-flex px-3 pt-2 justify-content-between align-items-center"lf><"bg-light m-0"rt><"card-footer d-flex px-3 pb-2 justify-content-between align-items-center"<"info-pag"i>p><"clear">'
});

let AllOpenAppointmentsPatient = new DataTable('#AllOpenAppointmentsPatient', {
    // options
    // scrollY: "300px",
    scrollCollapse: true,
    paging: true,
    select: true,
    "dom": '<"card-header d-flex px-3 pt-2 justify-content-between align-items-center"lf><"bg-light m-0"rt><"card-footer d-flex px-3 pb-2 justify-content-between align-items-center"<"info-pag"i>p><"clear">'
});

let AllPatientAppointments = new DataTable('#AllPatientAppointments', {
    // options
    // scrollY: "300px",
    scrollCollapse: true,
    paging: true,
    select: true,
    "dom": '<"card-header d-flex px-3 pt-2 justify-content-between align-items-center"lf><"bg-light m-0"rt><"card-footer d-flex px-3 pb-2 justify-content-between align-items-center"<"info-pag"i>p><"clear">'
});





// End DataTables

// Get Provider Messages

// function getProviderMessages(id) {
    // return new Promise((resolve, reject) => {
    //     $("#ProviderMessages").fadeIn('slow');
    //     document.getElementById("MessageBox").style.opacity = "1";

    //     $.when($.ajax(
    //         {
    //             url: `json/medicalnotes/${id}`,
    //             method: 'GET'
    //         })).done(function (data) {

    //             $('#First-Accordion-aHPI-ajax').html(data.hpi);
    //             $('#First-Accordion-aPE-ajax').html(data.pe);
    //             $('#First-Accordion-aSMRY-ajax').html(data.summary);
    //             $('#First-Accordion-aAP-ajax').html(data.ap);

    //             let apptid = data.appointmentId
                // Get appointment info
//                 $.when($.ajax(
//                     {
//                         url: `json/appointments/${apptid}`,
//                         method: 'GET'
//                     })).done(function (apptData) {
//                         $('#apptId-card-mn-ajax').html(apptData.appointmentId);
//                         let date = new Date(apptData.dateTime);
//                         $('#apptDate-card-mn-ajax').html(date.toDateString() + " at " + date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true }));
//                     });



//                 resolve();

//             }).fail(function (error) {
//                 reject(error);
//             });

//     });
// }

// get Patient Messages
function ShowMessages(writerId, providerId, patientId) {
    axios({
        method: "get",
        url: `/patientinbox/partial/${writerId}/${providerId}/${patientId}`,
    })
        .then((res) => {
            console.log(res);
            // var Inbox = createHtml(res.data);
            // document.getElementById("PtInbox").innerHTML(Inbox);
            $("#PtInbox").html(res.data);
            document.getElementById( 'bottom' ).scrollIntoView();
        })
        .catch((err) => {
            console.log(err);
        });
}

// update Patient Messages
function UpdateMessages(text, writerId, providerId, patientId) {
    axios({
        method: "get",
        url: `/updateinbox/partial/${text}/${writerId}/${providerId}/${patientId}`,
    })
        .then((res) => {
            console.log(res);
            // var Inbox = createHtml(res.data);
            // document.getElementById("PtInbox").innerHTML(Inbox);
            $("#PtInbox").html(res.data);
            document.getElementById( 'bottom' ).scrollIntoView();
        })
        .catch((err) => {
            console.log(err);
        });
}

// get Provider Messages
function ShowProviderMessages(writerId, providerId, patientId) {
    // element.classList.add("bg-yellow-300");
    // element.classList.add("fw-bold");
    // element.classList.add("ps-5");
    
    axios({
        method: "get",
        url: `/providerinbox/partial/${writerId}/${providerId}/${patientId}`,
    })
        .then((res) => {
            console.log(res);
            // var Inbox = createHtml(res.data);
            // document.getElementById("PtInbox").innerHTML(Inbox);
            $("#PrInbox").html(res.data);
            document.getElementById( 'bottom' ).scrollIntoView();
            
        })
        .catch((err) => {
            console.log(err);
        });
    
        
}


// update Provider Messages
function UpdateProviderMessages(text, writerId, providerId, patientId) {
    // element.classList.add("bg-yellow-300");
    // element.classList.add("fw-bold");
    // element.classList.add("ps-5");
    
    axios({
        method: "get",
        url: `/updateproviderinbox/partial/${text}/${writerId}/${providerId}/${patientId}`,
    })
        .then((res) => {
            console.log(res);
            $("#PrInbox").html(res.data);
            document.getElementById( 'bottom' ).scrollIntoView();
        })
        .catch((err) => {
            console.log(err);
        });
}

function patientJoinApp(element,AppointmentId){

    // disable button element
    element.disabled = true;
    // Element Html spinner
    element.innerHTML = '<i class="fa-solid fa-spinner fa-spin-pulse"></i>';
    // change element bg-color to success
    element.classList.remove("bg-teal-700");
    element.classList.add("btn-secondary");
    
    
    // send ajax request to join appointment
    axios({
        method: "POST",
        url: `/patient/joinappointment/${AppointmentId}`,
    })
        .then((res) => {
            console.log(res);
            setTimeout(function() {

            element.classList.remove("btn-secondary");
            element.classList.add("btn-success");
            // change element text to joined
            element.innerHTML = `Booked <i class="fa-solid fa-calendar-check"></i>`;
            // change element bg-color to success
            element.onclick = null;
            element.title = "You have joined this appointment";
            element.disabled = false;
            }, 2000);
            
            setTimeout(function()   {
                // reload page
                location.reload();
            }, 4000);
            
        })
        .catch((err) => {
            console.log(err);
        });
    
    
}

function patientUnJoinApp(element,AppointmentId){

    // disable button element
    element.disabled = true;
    // Element Html spinner
    element.innerHTML = '<i class="fa-solid fa-spinner fa-spin-pulse"></i>';
    // change element bg-color to success
    element.classList.remove("btn-danger");
    element.classList.add("btn-secondary");


    // send ajax request to cancel appointment
    axios({
        method: "POST",
        url: `/patient/unjoinappointment/${AppointmentId}`,
    })
        .then((res) => {
            console.log(res);
            setTimeout(function() {

                element.classList.remove("btn-secondary");
                element.classList.add("btn-success");
                // change element text to joined
                element.innerHTML = `Canceled <i class="fa-solid fa-calendar-check"></i>`;
                // change element bg-color to success
                element.onclick = null;
                element.title = "You have Canceled this appointment";
                element.disabled = false;
            }, 2000);

            setTimeout(function()   {
                // reload page
                location.reload();
            }, 4000);

        })
        .catch((err) => {
            console.log(err);
        });

}
function patientKnockRoom(element,AppointmentId){

    // disable button element
    element.disabled = true;
    // Element Html spinner
    element.innerHTML = '<i class="fa-solid fa-spinner fa-spin-pulse"></i>';
    // change element bg-color to success
    element.classList.remove("bg-orange-400");
    element.classList.add("btn-secondary");


    // send ajax request to cancel appointment
    axios({
        method: "POST",
        url: `/patient/joinappointment/watingroom/${AppointmentId}`,
    })
        .then((res) => {
            console.log(res);
            setTimeout(function() {

                element.classList.remove("btn-secondary");
                element.classList.add("btn-success");
                // change element text to joined
                element.innerHTML = `In the Waiting room <i class="fa-solid fa-circle-check"></i>`;
                // change element bg-color to success
                element.onclick = null;
                element.title = "You ara in waiting room";
                element.disabled = false;
            }, 2000);

            setTimeout(function()   {
                // reload page
                location.reload();
            }, 4000);

        })
        .catch((err) => {
            console.log(err);
        });

}


const element = document.getElementById('formsubmitnewMedPrescrib');
element.addEventListener('submit', event => {
    event.preventDefault();
    // actual logic, e.g. validate the form
    let form = document.getElementById("formsubmitnewMedPrescrib");
    let formAction = form.getAttribute("action");
    let formMethod = form.getAttribute("method");
    
    console.log(form["Name"]["value"]);
    // Using Axios js library to make the post request
    axios({
        method: 'POST',
        url: "/addmed",
        data: $(form).serialize(),
        dataType: 'json',
    })
        .then((res) => {
            // console.log(res);
            let list = document.getElementById("listOfMedPrescribed");
            let newelement = document.createElement("div");
            newelement.innerHTML = `<li class="list-group-item">
                            <span class="mb-1">${res.data.name}</span><br/>
                            <small class="text-muted fst-italic">
                                <span>${res.data.description}</span>
                            </small>
                        </li>`
            list.appendChild(newelement);
            
        })
        .catch((err) => {
            console.log(err);
        });
    
});





