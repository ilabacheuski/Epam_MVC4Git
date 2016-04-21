$('#get-quotes').submit(function (event) {
    event.preventDefault();
    var form = $(this);

    $.ajax({
        url: form.attr("action"),
        method: "POST",
        data: form.serialize(),
        beforeSend: function () {
            $("#ajax-loader").show();
        },
        complete: function () {
            $("#ajax-loader").hide();
        },
        error: searchFailed,
        success: function (data) {
            ParseJSON(data);
            
        }
    });
})

function searchFailed() { alert("Failed to request! Try again.") }

function ParseJSON(data)
{
    var html = Mustache.to_html($("#tableTemplate").html(),
            data);
    $("#data-table").empty().append(html);
    $('#quotesTable').DataTable();
}