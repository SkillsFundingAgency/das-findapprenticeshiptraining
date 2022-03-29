
// AUTOCOMPLETE

var $keywordsInput = $('#search-location');
var $submitOnConfirm = $('#search-location').data('submit-on-selection');
var $defaultValue = $('#search-location').data('default-value');
if ($keywordsInput.length > 0) {
    $keywordsInput.wrap('<div id="autocomplete-container" class="das-autocomplete-wrap"></div>');
    var container = document.querySelector('#autocomplete-container');
    var apiUrl = '/locations';
    $(container).empty();
    function getSuggestions(query, updateResults) {
        var results = [];
        $.ajax({
            url: apiUrl,
            type: "get",
            dataType: 'json',
            data: { searchTerm: query }
        }).done(function (data) {
            results = data.locations.map(function (r) {
                return r.name;
            });
            updateResults(results);
        });
    }
    function onConfirm() {
        var form = this.element.parentElement.parentElement;  
        var form2 = this.element.parentElement.parentElement.parentElement;
        setTimeout(function(){
            if (form.tagName.toLocaleLowerCase() === 'form' && $submitOnConfirm) {
                form.submit()
            }
            if (form2.tagName.toLocaleLowerCase() === 'form' && $submitOnConfirm) {
                form2.submit()
            }
    },200,form);}

    accessibleAutocomplete({
        element: container,
        id: 'search-location',
        name: 'location',
        displayMenu: 'overlay',
        showNoOptionsFound: false,
        minLength: 2,
        source: getSuggestions,
        placeholder: "",
        onConfirm: onConfirm,
        defaultValue: $defaultValue,
        confirmOnBlur: false,
        autoselect: true
    });
}


// FILTER CHECKBOXES
// If National filter is checked, then the parent 
// checkbox is also checked

$('#deliveryMode-National').on('change',function (){
    if ($(this).is(':checked')) {
        $('#deliveryMode-Workplace').prop('checked', true);
    }
});
$('#deliveryMode-Workplace').on('change', function(){
    if (!$(this).is(':checked')) {
        $('#deliveryMode-National').prop('checked', false);
    }
});


// BACK LINK
// If users history-1 does not come from this site, 
// then show a link to homepage

var $backLinkOrHome = $('.das-js-back-link-or-home');
var backLinkOrHome = function () {

    var referrer = document.referrer;

    var backLink = $('<a>')
        .attr({'href': '#', 'class': 'govuk-back-link'})
        .text('Back')
        .on('click', function (e) {
            window.history.back();
            e.preventDefault();
        });

    if (referrer && referrer !== document.location.href) {
        $backLinkOrHome.replaceWith(backLink);
    }
}

if ($backLinkOrHome) {
    backLinkOrHome();
}


// BACK TO TOP 
// Shows a back-to-top link in a floating header
// as soon as the breadcrumbs scroll out of view

$(window).bind('scroll', function() {

    var isCookieBannerVisible = $('.das-cookie-banner:visible').length,
        showHeaderDistance = 150 + (isCookieBannerVisible * 240),
        $breadcrumbs = $('.govuk-breadcrumbs');

    if ($breadcrumbs.length > 0) {
        var breadcrumbDistanceFromTop = $breadcrumbs.offset().top,
            breadcrumbHeight = $breadcrumbs.outerHeight();

        showHeaderDistance = breadcrumbDistanceFromTop + breadcrumbHeight;
    }

    if ($(window).scrollTop() > showHeaderDistance) {
        $('.app-shortlist-banner').addClass('app-shortlist-banner__fixed');
    } else {
        $('.app-shortlist-banner').removeClass('app-shortlist-banner__fixed');
    }

}).trigger("scroll");


// SCROLL TO TARGET 
// On click of the link, checks to see if the target exists
// If so, scrolls the page to that point, taking into account
// the back-to-top header

$("a[data-scroll-to-target]").on('click', function () {
    var target = $(this).data('scroll-to-target'),
        $target = $(target);
        headerOffset = $('.app-shortlist-banner__fixed').outerHeight() || 50;

    setTimeout(function() {
        if ($target.length > 0) {
            var scrollTo = $target.offset().top - headerOffset;
            $('html, body').animate({scrollTop: scrollTo}, 0);
        }
    }, 10)

});

// SHORTLIST

var shortlistAddForms = $('.app-provider-shortlist-add form');
var shortlistRemoveForms = $('.app-provider-shortlist-remove form');

var providerAddedClassName = 'app-provider-shortlist-added'

shortlistAddForms.on('submit', function(e) {
    var form = $(this);
    var formData = new FormData(this);
    formData.delete('routeName');
    sendData(formData, this.action, addFormDone, form);
    e.preventDefault();
});

shortlistRemoveForms.on('submit', function(e) {
    var form = $(this);
    var formData = new FormData(this);
    formData.delete('routeName');
    sendData(formData, this.action, removeFormDone, form);
    e.preventDefault();
});

var addFormDone = function(data, form) {
    var wrapper = form.closest('.app-provider-shortlist-control');
    var removeForm = wrapper.find('.app-provider-shortlist-remove form');
    removeForm.attr("action", "/shortlist/items/" + data);
    wrapper.addClass(providerAddedClassName)
    updateShortlistCount();
}

var removeFormDone = function(data, form) {
    var wrapper = form.closest('.app-provider-shortlist-control');
    var removeForm = wrapper.find('.app-provider-shortlist-remove form');
    removeForm.attr("action", "/shortlist/items/00000000-0000-0000-0000-000000000000");
    wrapper.removeClass(providerAddedClassName);
    updateShortlistCount(true);
}

var sendData = function(formData, action, doneCallBack, form){
    $.ajax({
        type: "POST",
        url: action,
        data: formData,
        processData: false,
        contentType: false
    }).done(function(data) {
        doneCallBack(data, form)
    });
}

var updateShortlistCount = function(remove) {
    var currentCount = $('body').data('shortlistcount');
    var shortlistCountsUi = $('.app-view-shortlist-link__number');
    
    currentCount += remove ? -1 : 1;

    $('body').data('shortlistcount', currentCount)
    shortlistCountsUi.text(currentCount).addClass('app-view-shortlist-link__number-update')

    setTimeout(function() {
        shortlistCountsUi.removeClass('app-view-shortlist-link__number-update')
    }, 1000);
}