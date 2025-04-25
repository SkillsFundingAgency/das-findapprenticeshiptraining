
// AUTOCOMPLETE

let $locationInput = $('#search-location');
let $submitOnConfirm = $('#search-location').data('submit-on-selection');
let $defaultValue = $('#search-location').data('default-value');
if ($locationInput.length > 0) {
    $locationInput.wrap('<div id="autocomplete-container" class="das-autocomplete-wrap"></div>');
    let container = document.querySelector('#autocomplete-container');
    let apiUrl = '/locations';
    $(container).empty();
    function getSuggestions(query, updateResults) {
        let results = [];
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
        let form = this.element.parentElement.parentElement;  
        let form2 = this.element.parentElement.parentElement.parentElement;
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

let $backLinkOrHome = $('.das-js-back-link-or-home');
let backLinkOrHome = function () {

    let referrer = document.referrer;

    let backLink = $('<a>')
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


// SHORTLIST

let shortlistAddForms = $('.app-provider-shortlist-add form');
let shortlistRemoveForms = $('.app-provider-shortlist-remove form');

let providerAddedClassName = 'app-provider-shortlist-added'

shortlistAddForms.on('submit', function(e) {
    let form = $(this);
    let formData = new FormData(this);
    formData.delete('routeName');
    sendData(formData, this.action, addFormDone, form);
    e.preventDefault();
});

shortlistRemoveForms.on('submit', function(e) {
    let form = $(this);
    let formData = new FormData(this);
    formData.delete('routeName');
    sendData(formData, this.action, removeFormDone, form);
    e.preventDefault();
});

let addFormDone = function(data, form) {
    let wrapper = form.closest('.app-provider-shortlist-control');
    let removeForm = wrapper.find('.app-provider-shortlist-remove form');
    removeForm.attr("action", "/shortlist/items/" + data);
    wrapper.addClass(providerAddedClassName);
    let courseProvider = form.closest('.app-course-provider')[0];
    let shortlistedTag = courseProvider.querySelector('.app-course-provider-shortlisted-tag')
    if (shortlistedTag) { shortlistedTag.style.display = 'block';  }
    updateShortlistCount(() => {
      let shortlistCountElement = document.querySelector('.app-view-shortlist-link__number');
      if (shortlistCountElement && parseInt(shortlistCountElement.textContent) >= 50) {
        // Find all elements with only class .app-provider-shortlist-control
        let elements = document.querySelectorAll('.app-provider-shortlist-control');
        elements.forEach(function (element) {
          if (element.classList.length === 1 && element.classList.contains('app-provider-shortlist-control')) {
            element.classList.add('app-provider-shortlist-full');
          }
        });
      }
    });

}

let removeFormDone = function(data, form) {
    let wrapper = form.closest('.app-provider-shortlist-control');
    let removeForm = wrapper.find('.app-provider-shortlist-remove form');
    removeForm.attr("action", "/shortlist/items/00000000-0000-0000-0000-000000000000");
    wrapper.removeClass(providerAddedClassName);
    let courseProvider = form.closest('.app-course-provider')[0];
    let shortlistedTag = courseProvider.querySelector('.app-course-provider-shortlisted-tag')
    if (shortlistedTag) { shortlistedTag.style.display = 'none'; }
    updateShortlistCount(() => {
      let shortlistCountElement = document.querySelector('.app-view-shortlist-link__number');
      if (shortlistCountElement && parseInt(shortlistCountElement.textContent) < 50) {
        // Find all elements with class .app-provider-shortlist-control and .app-provider-shortlist-full
        let elements = document.querySelectorAll('.app-provider-shortlist-control.app-provider-shortlist-full');
        elements.forEach(function (element) {
          element.classList.remove('app-provider-shortlist-full');
        });
      }
    });
}

let sendData = function(formData, action, doneCallBack, form){
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

let updateShortlistCount = function (updateAddRemoveLinks) {
  refreshComponent(updateAddRemoveLinks);
    let shortlistCountsUi = $('.app-view-shortlist-link__number');
    shortlistCountsUi.addClass('app-view-shortlist-link__number-update')

    setTimeout(function() {
      shortlistCountsUi.removeClass('app-view-shortlist-link__number-update')
    }, 1000);
}

function refreshComponent(updateAddRemoveLinks) {
    fetch('/shortlist/UpdateCount')
        .then(response => response.text())
        .then(html => {
            document.getElementById("ShortlistsCountContainer").innerHTML = html;
            if (updateAddRemoveLinks) {
              updateAddRemoveLinks();
            }
        });
}




// FEEDBACK GRAPH
// Generates the html to display
// feedback graph and populates the target

function nodeListForEach(nodes, callback) {
    if (window.NodeList.prototype.forEach) {
        return nodes.forEach(callback)
    }
    for (let i = 0; i < nodes.length; i++) {
        callback.call(window, nodes[i], i, nodes);
    }
}

function FeedbackGraph(table) {
    this.table = table
    this.target = this.table.dataset.target
    this.hideLegend = this.table.dataset.hideLegend === "true"
    this.label = this.table.dataset.label || "people"
    this.rows = this.table.querySelectorAll("tbody tr")
}

FeedbackGraph.prototype.init = function() {
    if (!document.getElementById(this.target)) {
        return
    }

    let that = this
    let rowCount = 0
    let legendlistHtml
    let graphHtml = document.createElement("div")
        graphHtml.className = "app-graph"

    let graphList = document.createElement("ul")
        graphList.className = "app-graph__list"

    let caption = this.table.querySelector("caption")
    let categoryHtml

    nodeListForEach(this.rows, function (row) {
        if (rowCount === 0) {
            legendlistHtml = that.legendHtml(row)
        }
        graphList.appendChild(that.graphRow(row))
        rowCount++
    });

    if (!this.hideLegend) {
        graphHtml.appendChild(legendlistHtml)
    }
    
    if (caption) {
        categoryHtml = document.createElement("h2")
        categoryHtml.className = "govuk-caption-l app-graph__category"
        categoryHtml.textContent = caption.textContent
        graphHtml.appendChild(categoryHtml)
    }

    graphHtml.appendChild(graphList)
    document.getElementById(this.target).appendChild(graphHtml)
}

FeedbackGraph.prototype.legendHtml = function(row) {
    let legendList = document.createElement('ul')
        legendList.className = "app-graph-key"
    let dataCells = row.querySelectorAll("td")
    let cellCount = 0

    if (this.hideLegend) {
        return ''
    }

    nodeListForEach(dataCells, function (dataCell) {
        if (isNaN(dataCell.textContent)) {

            let legendListItem = document.createElement("li")
                legendListItem.className = "app-graph-key__list-item app-graph-key__list-item--colour-" + (cellCount + 1)
                legendListItem.textContent = dataCell.dataset.label

            legendList.appendChild(legendListItem)
            cellCount++
        }
    });
    return legendList
}

FeedbackGraph.prototype.graphRow = function(row) {
    let that = this
    let questionText = row.querySelector("th").textContent
    let dataCells = row.querySelectorAll("td")
    let graphRowHtml = document.createElement('li')
    let barsHtml = document.createElement("div")
    let totalAsked = 0
    let barCount = 0

    graphRowHtml.className = "app-graph__list-item"
    barsHtml.className = "app-graph__chart-wrap"

    nodeListForEach(dataCells, function (dataCell) {
        if (isNaN(dataCell.textContent)) {
            let barHtml = that.barHtml(dataCell, barCount+1)
            if (barHtml !== undefined) {
                barsHtml.appendChild(barHtml)
            }
            barCount++
        } else {
            totalAsked = dataCell.textContent
        }
    });

    let caption = document.createElement('span')
        caption.className = "app-graph__caption"
        caption.textContent = "(selected by " + totalAsked + " " + this.label + ")"
    let heading = document.createElement('h3')
        heading.className = "app-graph__label"
        heading.textContent = questionText
        if (totalAsked > 0) {
            heading.appendChild(caption)
        }

        graphRowHtml.appendChild(heading)
        graphRowHtml.appendChild(barsHtml)

    return (graphRowHtml)
}

FeedbackGraph.prototype.barHtml = function(dataCell, barCount) {
    let percentage = parseFloat(dataCell.textContent.slice(0, -1));
    let span1 = document.createElement('span')
        span1.textContent = percentage + "%"
        span1.className = "app-graph__figure"

    let span2 = document.createElement('span')
        span2.className = "app-graph__bar-value app-graph__bar-value--colour-" + barCount
        span2.style.width = percentage + "%"
        span2.title = dataCell.dataset.title
        span2.tabIndex = 0
        span2.appendChild(span1)

    let span3 = document.createElement('span')
        span3.className = "app-graph__bar"
        span3.appendChild(span2)

    let span4 = document.createElement('span')
        span4.className = "app-graph__chart"
        span4.appendChild(span3)

    return(span4)

}



// SHOW/HIDE PANELS

function ShowHidePanels(container) {
    this.container = container
    this.panels = this.container.querySelectorAll('.app-show-hide-panel')
    this.hideClass = 'app-show-hide-panel__hidden'
}

ShowHidePanels.prototype.init = function() {
    let panelData = []
    let that = this
    nodeListForEach(this.panels, function (panel) {
        let panelObj = {
            id: panel.id,
            label: panel.dataset.panelLabel
        }
        panelData.push(panelObj)
        that.hidePanel(panel)
    })
    nodeListForEach(this.panels, function (panel) {
        panel.prepend(that.panelNav(panel.id, panelData))
    })
    this.showPanel(this.panels[0])
}

ShowHidePanels.prototype.panelNav = function(panelId, panelData) {
    let that = this
    let buttonWrap = document.createElement('div')
        buttonWrap.className = "govuk-button-group app-show-hide-panel__buttons"
    let filteredData = panelData.filter(function(item) {
        return item.id !== panelId
    })
    filteredData.forEach((item) => {
        buttonWrap.appendChild(that.showHideButton(item))
    });
    return buttonWrap
}

ShowHidePanels.prototype.showHideButton = function(item) {
    let button = document.createElement('a')
        button.className = 'govuk-button govuk-button--secondary govuk-!-font-size-16'
        button.textContent = 'Change to ' + item.label + ' view'
        button.href = '#' + item.id
        button.addEventListener('click', this.handleButtonClick.bind(this))
    return button
}

ShowHidePanels.prototype.handleButtonClick = function(e) {
    let that = this
    let targetPanel = e.target.hash.substring(1)
    nodeListForEach(this.panels, function (panel) {
        if (panel.id !== targetPanel) {
            that.hidePanel(panel)
        } else {
            that.showPanel(panel)
        }
    })
    e.preventDefault()
}

ShowHidePanels.prototype.hidePanel = function(panel) {
    panel.classList.add(this.hideClass)
}

ShowHidePanels.prototype.showPanel = function(panel) {
    panel.classList.remove(this.hideClass)
}


let showHidePanels = document.querySelectorAll('[data-show-hide-panels]');
nodeListForEach(showHidePanels, function (showHidePanel) {
  new ShowHidePanels(showHidePanel).init();
});

let feedbackGraphs = document.querySelectorAll('[data-feedback-graph]');
nodeListForEach(feedbackGraphs, function (feedbackGraph) {
  new FeedbackGraph(feedbackGraph).init();
});


let courseProvidersOrderBy = document.getElementById('course-providers-orderby');

courseProvidersOrderBy.addEventListener('change', function() {
   document.getElementById("course-providers-order-form").submit();
});


let buttonDeliveryModeProvider = document.getElementById('filteritem-modes-filter-Provider')
let buttonDeliveryModeDayRelease = document.getElementById('filteritem-modes-filter-DayRelease')
let buttonDeliveryModeBlockRelease = document.getElementById('filteritem-modes-filter-BlockRelease')

buttonDeliveryModeProvider.addEventListener('click', e => {
    if (buttonDeliveryModeProvider.checked) {
        buttonDeliveryModeDayRelease.checked = true;
        buttonDeliveryModeBlockRelease.checked = true;
    }

    if (!buttonDeliveryModeProvider.checked) {
        buttonDeliveryModeDayRelease.checked = false;
        buttonDeliveryModeBlockRelease.checked = false;
    }
});

buttonDeliveryModeDayRelease.addEventListener('click', e => {
    toggleDeliveryModeParent();
});

buttonDeliveryModeBlockRelease.addEventListener('click', e => {
    toggleDeliveryModeParent();
});

function toggleDeliveryModeParent() {
    if (!buttonDeliveryModeDayRelease.checked || !buttonDeliveryModeBlockRelease.checked) {
        buttonDeliveryModeProvider.checked = false;
    }

    if (buttonDeliveryModeDayRelease.checked || buttonDeliveryModeBlockRelease.checked) {
        buttonDeliveryModeProvider.checked = true;
    }
}


function toggleTables() {


    var prefix =  'app';

    // Select all graph panels and table panels based on the prefix
    var graphPanels = document.querySelectorAll(`.app-show-hide-panel[data-panel-label="graph"][id^="${prefix}-feedback-graph-"]`);
    var tablePanels = document.querySelectorAll(`.app-show-hide-panel[data-panel-label="table and accessible"][id^="${prefix}-feedback-table-"]`);


    // Toggle visibility for all graph panels
    var graphVisible = false;
    for (var i = 0; i < graphPanels.length; i++) {
        graphPanels[i].classList.toggle("app-show-hide-panel__hidden");
        if (!graphPanels[i].classList.contains("app-show-hide-panel__hidden")) {
            graphVisible = true;
        }
    }

    // Toggle visibility for all table panels
    for (var i = 0; i < tablePanels.length; i++) {
        tablePanels[i].classList.toggle("app-show-hide-panel__hidden");
    }

    var prefixEmp = 'emp';

    // Select all graph panels and table panels based on the prefix
    var graphPanelsEmp = document.querySelectorAll(`.app-show-hide-panel[data-panel-label="graph"][id^="${prefixEmp}-feedback-graph-"]`);
    var tablePanelsEmp = document.querySelectorAll(`.app-show-hide-panel[data-panel-label="table and accessible"][id^="${prefixEmp}-feedback-table-"]`);


    // Toggle visibility for all graph panels
    var graphVisibleEmp = false;
    for (var i = 0; i < graphPanelsEmp.length; i++) {
        graphPanelsEmp[i].classList.toggle("app-show-hide-panel__hidden");
        if (!graphPanelsEmp[i].classList.contains("app-show-hide-panel__hidden")) {
            graphVisibleEmp = true;
        }
    }

    // Toggle visibility for all table panels
    for (var i = 0; i < tablePanelsEmp.length; i++) {
        tablePanelsEmp[i].classList.toggle("app-show-hide-panel__hidden");
    }


    var analyticsConsentName = "AnalyticsConsent" + getEnvFromHost();
    var analyticsConsentChoice = getCookie(analyticsConsentName);

    // Save user preference in a cookie conditional to the analytics consent
    if (analyticsConsentChoice === "true") {
        setCookie('viewPreference-apprentice', graphVisible ? 'graph' : 'table', 1);
        setCookie('viewPreference-employer', graphVisibleEmp ? 'graph' : 'table', 1);
    }
}


function getEnvFromHost() {
    var host = window.location.host;

    var env = "";

    if (host.includes("at-")) {
        env = "AT";
    } else if (host.includes("test-")) {
        env = "TEST";
    } else if (host.includes("test2-")) {
        env = "TEST2";
    } else if (host.includes("pp-")) {
        env = "PP";
    }

    return env;
}