$(document).ready(function () {

    $.ajax({
        url: $('#btn-search').attr("action") + window.location.search,
        method: 'GET',
        beforeSend: function () {
            $("#ajax-wait").show();
        },
        complete: function () {
            $("#ajax-wait").hide();
        },
        error: searchFailed,
        success: parseTable
        
    })

    function parseTable(response) {
        var html = Mustache.to_html($("#mst-template-table").html(), response);
        $("#table").empty().append(html);
        $('#mst-table-data').DataTable();
    }

    //$('#get-quotes').submit(function (event) {
    //    event.preventDefault();
    //    var form = $(this);

    //    $.ajax({
    //        url: form.attr("action"),
    //        method: 'GET',
    //        data: form.serialize(),
    //        beforeSend: function () {
    //            $("#ajax-loader").show();
    //        },
    //        complete: function () {
    //            $("#ajax-loader").hide();
    //        },
    //        error: searchFailed,
    //        success: function (data) {
    //            ParseJSON(data);
    //            BindButtonClick();
    //        }
    //    });
    //})

    function searchFailed() { alert("Failed to request! Try again.") }

    function ParseJSON(data)
    {
        var html = Mustache.to_html($("#tableTemplate").html(),
                data);
        $("#data-table").empty().append(html);
        $('#quotesTable').DataTable();
    }

    $('#form-quotes').submit(function (event) {
        event.preventDefault();
        var form = $(this);
        var url = $('#btn-search').attr("action");
        $.ajax({
            url: url,
            data: form.serialize(),
            beforeSend: function () {
                $("#ajax-loader").show();
            },
            complete: function () {
                $("#ajax-loader").hide();
            },
            error: searchFailed,
            success: function (response)
            {
                parseTable(response);
                // location.href = "?" + form.serialize();
                
            }
        })
    })
})

function BindButtonClick() {
    $("button").click(function (e) {
        var idClicked = e.target.value;

        var form = $("#get-quotes");
        var ActionUrl = $("#export-form").attr("action");

        $.ajax({
            url: ActionUrl,
            method: 'GET',
            data: form.serialize() + "&SelectedFormat=" + idClicked
        })
    })
}