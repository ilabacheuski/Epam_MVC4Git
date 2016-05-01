$(document).ready(function () {

    //$.ajax({
    //    url: $('#btn-search').attr("action") + window.location.search,
    //    method: 'GET',
    //    beforeSend: showAjaxLoader,
    //    complete: hideAjaxLoader,
    //    error: searchFailed,
    //    success: parseTable
        
    //})

    $('#form-quotes').submit(function (event) {
        event.preventDefault();
        var form = $(this);
        var url = $('#btn-search').attr("action");
        var data_ = form.serialize();
        $.ajax({
            url: url,
            data: data_,
            beforeSend: showAjaxLoader,
            complete: hideAjaxLoader,
            error: searchFailed,
            success: function (response)
            {
                parseTable(response);
                bindExportBtn();
                
                var provider = $('#ProviderName').val();
                var stateObj = { indexPage: "Quotes_" + provider };
                history.pushState(stateObj, "Quotes " + provider, "?" + data_);
            }
        })
    })


})


function parseTable(response) {
    var html = Mustache.to_html($("#mst-template-table").html(), response);
    $("#table").empty().append(html);
    $('#mst-table-data').DataTable();
}

function searchFailed() { alert("Failed to request! Try again.") }

function bindExportBtn() {
    $('.export-btn').click(function (event) {
        event.preventDefault();
        var idClicked = event.target.value;
        var form = $('#form-quotes');
        var ActionUrl = $('.export-btn').attr("action");
        window.location = ActionUrl + "?" + form.serialize() + "&SelectedFormat=" + idClicked;
    })
}

function showAjaxLoader() {
    $("#ajax-wait").show();
}
function hideAjaxLoader() {
    $("#ajax-wait").hide();
}