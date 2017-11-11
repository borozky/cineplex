// Write your Javascript code.
var element = $(".slider")[0];
window.mySwipe = new Swipe(element, {
    startSlide: 0,
    auto: 3000,
    draggable: true,
    autoRestart: false,
    continuous: true,
    disableScroll: true,
    stopPropagation: true,
    callback: function (index, element) { },
    transitionEnd: function (index, element) { }
});

$(document).ready(function () {
    $(".datetime-picker").datetimepicker({
        format: "MM/DD/YYYY hh:mm:ss a"
    });

    var movies = new Bloodhound({
        datumTokenizer:function(data){
            return Bloodhound.tokenizers.whitespace(data.movies.value);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: "/api/Movies/Search/Title/%QUERY",
            wildcard: "%QUERY",
            filter: function (response) {
                return response.data.movies;
            }
        }
    });

    movies.initialize();

    $(".typeahead").typeahead(null, {
        name: 'movies',
        displayKey: function(movies){
            return movies.title;
        },
        source: movies.ttAdapter(),
        suggestion: function (data) {
            return '<p class="something"><strong>' + data.value + '</p>';
        }
      });


});