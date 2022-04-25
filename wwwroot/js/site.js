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
