// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.









$(function () {
    var placeholderElement = $('#modal-placeholder-aggvalutazione');
    $('button[data-toggle="ajax-modal-aggvalutazione"]').click(function (event) {
        var geturl = `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}`;
        var urldettaglio = '/ApplicazioniRischi/IndexApplicazioniRischi?handler=AggiungiModalePartial';
        geturl += urldettaglio;
        $.get(geturl).done(function (data) {
            // append HTML to document, find modal and show it
            //$(document.body).append(data).find('.modal').modal('show');
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});



$(function () {
    var placeholderElement = $('#modal-placeholder-assegnazioni');
    $('button[data-toggle="ajax-modal-modassegnazione"]').click(function (event) {
        var geturl = `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}`;
        var urldettaglio = '/Attività/AttivitàPersone?handler=ModAssegnazioniModalePartial';
        geturl += urldettaglio;
        const idriga = $(this).attr('data-idriga');
        geturl += "&idriga=";
        geturl += idriga;
        const idattivita = $(this).attr('data-idattivita');
        geturl += "&idattivita=";
        geturl += idattivita;
        const idpersona = $(this).attr('data-idpersona');
        geturl += "&idpersona=";
        geturl += idpersona;
        $.get(geturl).done(function (data) {
            // append HTML to document, find modal and show it
            //$(document.body).append(data).find('.modal').modal('show');
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});

$(function () {
    var placeholderElement = $('#modal-placeholder-assegnazioni');
    $('button[data-toggle="ajax-modal-aggassegnazione"]').click(function (event) {
        var geturl = `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}`;
        var urldettaglio = '/Attività/AttivitàPersone?handler=ModAssegnazioniModalePartial';
        geturl += urldettaglio;
        const idattivita = $(this).attr('data-idattivita');
        geturl += "&idattivita=";
        geturl += idattivita;
        $.get(geturl).done(function (data) {
            // append HTML to document, find modal and show it
            //$(document.body).append(data).find('.modal').modal('show');
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});

$(function () {
    var placeholderElement = $('#modal-placeholder-assegnazioni');
    $('button[data-toggle="ajax-modal-modwp"]').click(function (event) {
        var geturl = `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}`;
        var urldettaglio = '/Attività/AttivitàWPs?handler=ModWPModalePartial';
        geturl += urldettaglio;
        const idattivita = $(this).attr('data-idattivita');
        geturl += "&idattivita=";
        geturl += idattivita;
        const idwp = $(this).attr('data-idwp');
        geturl += "&idwp=";
        geturl += idwp;
        $.get(geturl).done(function (data) {
            // append HTML to document, find modal and show it
            //$(document.body).append(data).find('.modal').modal('show');
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});

$(function () {
    var placeholderElement = $('#modal-placeholder-assegnazioni');
    $('button[data-toggle="ajax-modal-aggwp"]').click(function (event) {
        var geturl = `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}`;
        var urldettaglio = '/Attività/AttivitàWPs?handler=ModWPModalePartial';
        geturl += urldettaglio;
        const idattivita = $(this).attr('data-idattivita');
        geturl += "&idattivita=";
        geturl += idattivita;
        $.get(geturl).done(function (data) {
            // append HTML to document, find modal and show it
            //$(document.body).append(data).find('.modal').modal('show');
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});