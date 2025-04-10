function saveAsFile(filename, base64Data) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:application/pdf;base64,' + base64Data);
    element.setAttribute('download', filename);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
}

function saveAsExcel(filename, bytesBase64) {
    if (navigator.msSaveBlob) {
        //Descarga el archivo en el navegador de edge
        var data = window.atob(bytesBase64);
        var bytes = new Uint8Array(data.length);
        for (var i = 0; i < data.length; i++) {
            bytes[i] = data.charCodeAt(i);
        }
        var blob = new Blob([bytes.buffer], { type: "application/octet-stream" });
        navigator.msSaveBlob(blob, filename);
    }
    else {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Se requiere para firefox
        link.click();
        document.body.removeChild(link);
    }
}

function saveAsZip(filename, base64Data) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:application/zip;base64,' + base64Data);
    element.setAttribute('download', filename);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
}

function saveAsTxt(filename, base64Data) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;base64,' + base64Data);
    element.setAttribute('download', filename);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
}

function allowOnlyNumbers(event) {
    var charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        event.preventDefault();
    }
}