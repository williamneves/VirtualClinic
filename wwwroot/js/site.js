// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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

// Auto Save Medical Notes
// Set Interval function variable
let auto_refresh;

// Catch the form submit forms by .text-area-notes
document.querySelectorAll('.text-area-notes').forEach(function(element) {
    // Add event listener Focus to each element, to catch the focus event and start the auto save
    element.addEventListener('focus', function(evt) {
        autosaveForm(true);
    });
    
    // Add event listener Blur to each element, to catch the blur event and stop the auto save
    element.addEventListener('blur', function(evt) {
        autosaveForm(false);
    });
});



// Auto Save Function 
// Set parameter run to true or false, set parameter FormID to the ID of the form, set the parameter interval to the interval of the auto save
function autosaveForm(run = false,FormID = "form-medical-note-pd", interval = 500 ) {
    
    // If run is true, start the auto save
    if (run) {
    auto_refresh = setInterval(
        function()
        {
            post();
        }, interval);
    }
    // If run is false, stop the auto save
    else {
        clearInterval(auto_refresh);
    }
    
    // Function to Save the form (if is new, backend will create a new record, if is update, backend will update the record)
    function post(){
        
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
// autosize(document.querySelectorAll('textarea'));

$('.text-area-notes').each(function(){
    autosize(this);
}).on('autosize:resized', function(){
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
// // }



