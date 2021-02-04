$(document).ready(function () {
    Base.call(MapEgulf);
    $('.content, .container-fluid').addClass('p-0');
    $('html').css('height', '100%');
    $('body').css('height', '100%');
    MapEgulf.Load(MapEgulf.options);
    //$('#date-range').datepicker({
    //    toggleActive: true
    //});
});


var MapEgulf = {
    $container: $('#map-container'),
    options: {
        center: [18.654674, -95.787245],
        zoom: 12,
        zoomControl: false
    },

    vars: {
        url: {
            regions: '/map/regions',
            vessels: '/map/vessels',
            offerTransaction: '/match/offertransaction',
            select: '/match/select',

        },
        tags: {
            OK: "Correcto",
            Saved: "Registrado correctamente",
            SomethingWrong: "Ha ocurrido un error",
            Warning: "Aviso",
            ErrorForm: "Verifica la información",
            Acept: "Aceptar",
            Cancel: "Cancelar",
            AcceptOffer: "Aceptar oferta",
            AcceptText: "¿Desea aceptar la oferta? Esto generará un informe de aceptación. ",
            Fix: "Negociar oferta",
            FixText: "Este acción permitirá negociar la oferta mediante un chat.",
            Reject: "Rechazar oferta",
            RejectText: "¿Desea rechazar la oferta?",
        },

        ctrlZoomPosition: 'bottomright',
        //useSideBar: true,
        //sidebarPosition: 'left',
        isLoaded: false,
        regions: [],
        vessels: [],
        layerMarkers: new L.MarkerClusterGroup(),
        //editablePolylines: new L.FeatureGroup([]),
        map: null,
        colors: ['#FF5733', '#3393FF', '#6BFF33', '#FF3383'],

    },

    Load: function (options) {
        try {
            var options = options || this.options;
            var map = L.map('map-container', {
                zoomControl: false,
                doubleClickZoom: true,
            }).on('load', function (e, context) {
                this.vars.isLoaded = true;
                $(document).trigger('maploaded');
            }, this).setView(this.options.center, 6);
            this.SetMap(map);
            //nuevos distros de la version 1.3 de leaflet
            let osmUrl = 'http://{s}.tile.osm.org/{z}/{x}/{y}{r}.png';
            let osmAttrib = '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors';
            L.tileLayer(osmUrl, { minZoom: 1, maxZoom: 20, attribution: osmAttrib, detectRetina: true }).addTo(map);
            L.control.zoom({ //boton de zoom
                position: this.vars.ctrlZoomPosition
            }).addTo(map);
            //this.LoadRegions();
            this.intSideBar();
            //this.LoadVessels();

        } catch (e) {
            console.log(e);
        }
    },

    intSideBar: function () {
        let map = this.GetMap();
        // create the sidebar instance and add it to the map
        var sidebar = L.control.sidebar({ container: 'sidebar' })
            .addTo(map)
            .open('home');


        // be notified when a panel is opened
        sidebar.on('content', function (ev) {
            switch (ev.id) {
                case 'autopan':
                    sidebar.options.autopan = true;
                    break;
                default:
                    sidebar.options.autopan = false;
            }
        });

        $('#profileLink').on('click', function () {
            sidebar.close();
        });
    },

    isLoaded: function () {
        return this.vars.isLoaded;
    },

    SetMap: function (map) {
        this.vars.map = map
    },

    Center: function (e) {
        let mapa = this.GetMap();
        mapa.panTo(e.latlng);
    },

    ZoomIn: function (e) {
        let mapa = this.GetMap();
        mapa.zoomIn();
    },

    ZoomOut: function (e) {
        let mapa = this.GetMap();
        mapa.zoomOut();
    },

    GetMap: function () {
        return this.vars.map;
    },

    SetRegions: function (regions) {
        this.vars.regions = regions;
    },

    GetRegions: function () {
        return this.vars.regions;
    },

    SetVessels: function (regions) {
        this.vars.regions = regions;
    },

    GetVessels: function () {
        return this.vars.regions;
    },

    AddToCluster: function (marker) {
        this.vars.layerMarkers.addLayer(marker);
    },

    GetClusters: function () {
        return this.vars.layerMarkers;
    },

    LoadRegions: function () {
        this.Ajax(this.vars.url.regions, 'POST', null, true, function (data, ctx) {
            if (data != null) {
                ctx.SetRegions(data);
                ctx.RenderRegions();
            }
            else
                ctx.Toast("warning", "top rigth", "Error");
        }, function (xhr, textStatus, ctx) {

        })
    },

    LoadVessels: function () {
        this.Ajax(this.vars.url.vessels, 'POST', null, true, function (data, ctx) {
            if (data != null) {
                ctx.SetVessels(data);
                ctx.RenderVessels();
            }
            else
                ctx.Toast("warning", "top rigth", "Error");
        }, function (xhr, textStatus, ctx) {

        })
    },

    RenderRegions: function () {
        let regs = this.GetRegions();
        let colors = this.vars.colors;
        regs.forEach((obj, i) => {
            let coord = [];
            obj.Coordenates.forEach((obj, i) => {
                coord.push(obj.ToLatLng);
            });
            var polygon = L.polygon(coord, {
                color: '#FF5733',
                fillColor: '#FF5733',
                fillOpacity: 0.5,
            }).addTo(this.GetMap());
        });
    },

    RenderVessels: function () {
        let vess = this.GetVessels();
        LeafIcon = L.Icon.extend({
            options: {
                iconUrl: '/Content/Images/Pin2.png',
                iconSize: [25, 32],
            }
        });
        vess.forEach((obj, i) => {
            //var html = '<div class="" style="width:230px">' +
            //            '<div class="row m-b-5">' +
            //                '<div class="col-md-10">' +
            //                    '<h5 class="card-title text-primary font-weight-bold">' + obj["Name"] + '</h5>' +
            //                '</div>' +
            //                '<div class="col-md-2">' +
            //                    '<h5 class="card-title font-weight-bold">MX</h5>' +
            //                '</div>' +
            //            '</div>' +
            //             '<div class="row m-b-5">' +
            //                '<div class="col-md-1"><img src="/Content/Images/calendar.png"></div>' +
            //                '<div class="col-md-10">'+obj["YearBuild"]+'</div>' +
            //            '</div>' +
            //             '<div class="row m-b-5">' +
            //                '<div class="col-md-1"><img src="/Content/Images/calendar.png"></div>' +
            //                '<div class="col-md-10 txtDisponibilidad">{Disponibilidad}</div>' +
            //            '</div>' +
            //            '<div class="row m-b-5">' +
            //                '<div class="col-md-1"><img src="/Content/Images/calendar.png"></div>' +
            //                '<div class="col-md-10 txtDisponibilidad">{Disponibilidad}</div>' +
            //            '</div>' +
            //            '<div class="row">' +
            //                '<div class="col-md-1"><img src="/Content/Images/pin_location.png"></div>' +
            //                '<div class="col-md-10 txtUbicacion" >' +'{Ubicacion}' + '</div>' +
            //            '</div>' +
            //            '</div>';
            //console.log(obj);

            var vesselIcon = new LeafIcon();
            let coord = [];
            if (obj.Location != null)
                coord = obj.Location.ToLatLng;
            else
                coord = [18.684764, -91.813161];
            var m = L.marker(coord, { icon: vesselIcon }).bindPopup("");
            this.AddToCluster(m);
            obj.marker = m;
        });
        this.GetMap().addLayer(this.GetClusters());
        this.SetVessels(vess);
        //this.RemoveVesselsMarker();
    },

    RemoveVesselsMarker: function () {
        this.GetMap().removeLayer(this.GetClusters());
        this.vars.layerMarkers = new L.MarkerClusterGroup();
        //let vess = this.GetVessels();
        //vess.forEach((obj, i) => {
        //    this.GetMap().removeLayer(obj.marker);
        //});
    },

    RenderVesselsMarch: function (dataMatch, hasPrject = false) {
        this.RemoveVesselsMarker();
        this.SetVessels(dataMatch)
        let vess = this.GetVessels();
        LeafIcon = L.Icon.extend({
            options: {
                iconUrl: '/Content/Images/Pin2.png',
                iconSize: [25, 32],
            }
        });
        LeafIcon2 = L.Icon.extend({
            options: {
                iconUrl: '/Content/Images/Pin21.png',
                iconSize: [25, 32],
            }
        });
        let arrLatLng = [];
        vess.forEach((obj, i) => {

            var Vessel = obj;
            var VesselData = Vessel.VesselMatch;
            var Name = VesselData.Name;
            var Country = VesselData.Country.Name;
            var YearBuild = VesselData.YearBuild;
            var VesselType = VesselData.VesselType.Name;
            var Suitability = Vessel.VesselSuitabilityProject;
            var Port = VesselData.HomePort.Name;
            var IMO = VesselData.Imo;
            var DPS = Vessel.DynamicPositionSystemName;
            var BHP = Vessel.BHP;
            var SubCategory = Vessel.Subtype;
            var BollardPull = Vessel.BollardPull;
            var BollardPullAhead = Vessel.BollardPullAhead;
            var BollardPullAstern = Vessel.BollardPullAstern;
            var NumberPassengers = Vessel.NumberPassengers;
            var CargoCapacity = Vessel.CargoCapacity;
            var PumpRates = Vessel.PumpRates;

            var customPopup =
                `<div class="" style="width:auto;min-width:250px;">
                    <div class="row m-b-5">
                        <div class="col-md-1"><h4><i class="fa fa-flag text-primary"></i></h4></div>
                        <div class="col-md-10"><b>Bandera: </b>${Country}</div>
                    </div>
                    <div class="row m-b-5">
                        <div class="col-md-1"><h4><i class="fa fa-calendar text-primary"></i></h4></div>
                        <div class="col-md-10"><b>Año de Construcción: </b>${YearBuild}</div>
                    </div>
                     <div class="row m-b-5">
                        <div class="col-md-1"><h4><i class="fa fa-ship text-primary"></i></h4></div>
                        <div class="col-md-10"><b>Tipo Embarcación: </b>${VesselType}</div>
                    </div>
                   <div class="row">
                        <div class="col-md-1"><h4><i class="fa fa-map-pin text-primary"></i></h4></div>
                        <div class="col-md-10"><b>Puerto base: </b>${Port}</div>
                   </div>`;

            if (hasPrject) {
                customPopup += `<hr>
                    
                    ${BHP?
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>BHP:${BHP} </b></div>
                    </div>`: ``}
                    ${DPS ? `
                    <div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Sistema Posicionamiento Dinámico: </b>${DPS}</div>
                    </div>` : ``}
                    ${SubCategory ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Subcategoría: </b>${SubCategory}</div>
                    </div>` : ``}
                    ${BollardPull ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Tirón a Punto Fijo: </b>${BollardPull}</div>
                    </div>` : ``}
                    ${BollardPullAhead ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Tirón a Proa: </b>${BollardPullAhead}</div>
                    </div>` : ``}
                    ${BollardPullAstern ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Tirón a Popa: </b>${BollardPullAstern}</div>
                    </div>` : ``}
                    ${NumberPassengers ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Capacidad de Pasajeros: </b>${NumberPassengers}</div>
                    </div>` : ``}
                    
                    ${CargoCapacity ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Capacidad de Carga: </b>${ CargoCapacity}</div>
                    </div>` : ``}
                    ${PumpRates ? 
                    `<div class="row">
                        <div class="col-md-1"><h5><i class="fa fa-info-circle text-primary"></i></h5></div>
                        <div class="col-md-10"><b>Capacidad de Descarga: </b>${PumpRates}</div>
                    </div>` : ``}`;
        }

        customPopup = customPopup +
                '<div class="row">' +
                '<div class="col-sm-12">';
            if (hasPrject && !obj.IsMyVessel) {
                if (obj.Offerted === 0)
                    customPopup += '<input id="btnOffer' + obj.MatchableId + '" onclick="MapEgulf.Offer(' + obj.MatchableId + ')" type="button" value="Ofertar" class="btn btn-primary btn-sm pull-right" />';

                customPopup += '<input id="btnOffered' + obj.MatchableId + '" type="button" value="Ofertado" class="btn btn-sm pull-right disabled" style="' + (obj.Offerted === 0 ? 'display: none' : '') + '" />';
            }
            customPopup += '</div>' +
                '</div>' +
                '</div>';

            var customOptions =
            {
                'width': '230px',
            }

            var vesselIcon = obj.IsMyVessel ? new LeafIcon2 : new LeafIcon();
            let coord = [];
            if (obj.VesselMatch.Location != null)
                coord = obj.VesselMatch.Location.ToLatLng;
            else
                coord = [18.654674, -95.787245];
            arrLatLng.push(coord);
            var m = L.marker(coord, { icon: vesselIcon }).bindPopup(customPopup, customOptions);
            this.AddToCluster(m);
            obj.marker = m;


        });
        //var bounds = new L.LatLngBounds(arrLatLng);
        let zoomBound = this.GetMap().getBoundsZoom(this.GetClusters().getBounds())
        console.log(zoomBound);
        if (zoomBound < 15)
            this.GetMap().addLayer(this.GetClusters()).fitBounds(this.GetClusters().getBounds());
        else
            this.GetMap().addLayer(this.GetClusters()).fitBounds(this.GetClusters().getBounds()).setZoom(10);
        this.SetVessels(vess);
        //this.RemoveVesselsMarker();

    },

    Offer: function (VesselId) {

        Match.BlockUI("#sidebar");
        Match.BlockUI("#map-container");

        let objForm = {
            "Vessel.VesselId": VesselId,
            "Project.ProjectId": $("#projects").val()
        };

        this.Ajax(this.vars.url.offerTransaction + "/offer", 'POST', objForm, true, function (data, ctx) {
            if (data !== null && data.Status === 0) {

                MapEgulf.RefreshOffer(VesselId);
                //Update offer
                Match.LoadOfferts();
                Match.UnblockUI("#map-container");
            }
            else {
                Match.BlockUI("#sidebar");
                Match.UnblockUI("#map-container");
                ctx.Modal.Oops(data.Message);
                ctx.Toast("warning", "top rigth", "Error", data.Message ? data.Message : "Error al ofertar");
            }
        });
    },

    RefreshAvailablyOffers: function (id) {
        $("#projects option[value='" + id + "']").remove();
    },

    Transaction: function (oper, offerId) {
        Match.BlockUI("#sidebar");

        let objForm = {
            "OfferId": offerId
        };

        this.Ajax(this.vars.url.offerTransaction + "/" + oper, 'POST', objForm, true, function (data, ctx) {
            Match.UnblockUI("#sidebar");

            if (data !== null && data.Status === 0) {
                ctx.Toast("success", "top rigth", ctx.vars.tags.OK, data.Message ? data.Message : ctx.vars.tags.Saved);
                if (oper === "ACCEPT") {
                    Match.LoadOfferts();
                }
                else if (oper === "FIX") {
                    $("#lichat").removeClass("disabled");
                    $('#btnChatOffer[data-OfferId="' + offerId + '"]').attr("data-status", "5");
                    $("#mchat").load("/chat/partial?ReferenceId=" + offerId, function () { Match.OnResize(); });
                    $("#lichat").addClass("active");
                    $("#autopan").removeClass("active");
                    $("#chat").addClass("active");
                    Match.BlockUI("#sidebar");

                }
                else if (oper === "REJECT") {
                    $("#btnGroup" + offerId).remove();
                    $("#icoStatusRej" + offerId).show();
                }

            }
            else
                ctx.Toast("warning", "top rigth", ctx.vars.tags.Warning, data.Message ? data.Message : ctx.vars.tags.SomethingWrong);
        });
    },

    ValTransaction: function (oper, offerId, receibedOffer = true) {
        if (oper === "FIX") {
            
            let status = $('#btnChatOffer[data-OfferId="' + offerId + '"]').attr("data-status");
            $('#btnChatOffer[data-OfferId="' + offerId + '"]').parent().removeClass("btn-warning");
            if (status === "5") {
                Match.BlockUI("#sidebar");
                $("#mchat").load("/chat/partial?ReferenceId=" + offerId, function () { Match.OnResize(); });
                $("#lichat").addClass("active");
                $("#autopan").removeClass("active");
                $("#chat").addClass("active");
                return;
            }
        }
        if (oper === "ACCEPT" && !receibedOffer) {
            $('#aships').click();
            $("#autopan").removeClass("active");
            $("#liships").addClass("active");
            $('#asign').addClass("active")
            let offer = Match.vars.offers.find(obj => { return obj.OfferId == offerId });
            Match.MapVesselToOffer({ company: offer.Vessel.Company.CompanyId, project: offer.Project.ProjectId, offer: offer.OfferId });
        }
        else {
            MapEgulf.Modal.Generic("warning",
                oper === "ACCEPT" ? MapEgulf.vars.tags.AcceptOffer : oper === "FIX" ? MapEgulf.vars.tags.Fix : MapEgulf.vars.tags.Reject,
                oper === "ACCEPT" ? MapEgulf.vars.tags.AcceptText : oper === "FIX" ? MapEgulf.vars.tags.FixText : MapEgulf.vars.tags.RejectText,
                MapEgulf.vars.tags.Accept,
                MapEgulf.vars.tags.Close,
                function () { MapEgulf.Transaction(oper, offerId); });
        }
    },

    RefreshOffer: function (vesselId) {
        //Update button
        $("#btnOffer" + vesselId).remove();
        $("#btnOffered" + vesselId).show();
    },

    RefreshChat: function (offerId) {
        $('#btnChatOffer[data-OfferId="' + offerId + '"]').parent().css("display", "block");
        $('#btnChatOffer[data-OfferId="' + offerId + '"]').attr("data-Status", "5");
    },






}
