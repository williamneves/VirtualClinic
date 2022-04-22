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
