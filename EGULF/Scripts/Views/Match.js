$(document).ready(function () {
    Base.call(Match);
    Match.Init();

});

var Match = {
    resources: {
        erro_load_project: "No se encontraron coincidencias"
    },
    vars: {
        offers: [],
        $project: $('#projects'),
        $suitability: $('#suitability'),
        $region: $('#region'),
        $pemex: $('#pemex'),
        $fStart: $('#fStart'),
        $fEnd: $('#fEnd'),
        $cost: $('#costs'),
        $systemPos: $("input[name='postionSystem']"),
        $subtype: $("#subtype"),
        $pasajeros: $("#pasajeros"),
        $carga: $("#carga"),
        $b_pull: $('#b_pull'),
        $b_pull_ahead: $('#b_pull_ahead'),
        $b_pull_astern: $('#b_pull_astern'),

        $offerFilter: $('#offerFilter'),
        $offerObjectIcon: $('#offerObjectIcon'),
        $txtOwner: $('#txtOwner'),
        $txtType: $('#txtType'),
        $txtIMO: $('#txtIMO'),
        $txt_date: $('#txt-date'),
        $icoStatus: $('#icoStatus'),
        $icoDir: $("#icoDir"),
        $txtPort: $('#txtPort'),
        $viewDetail: $('#viewDetail'),
        $btnAcceptOffer: $('#btnAcceptOffer'),
        $btnCancelOffer: $('#btnCancelOffer'),
        $btnChatOffer: $('#btnChatOffer'),

        tags: {
            OK: "Correcto",
            Saved: "Registrado correctamente",
            SomethingWrong: "Ha ocurrido un error",
            Warning: "Aviso",
            ErrorForm: "Verifica la información",
            Acept: "!Aceptar",
            Cancel: "!Cancelar",
        },

        url: {
            project: "/match/project",
            find: "/match/Find",
            offers: "/match/offers",
            offerTransaction: '/match/offertransaction',
            vesselsToOffer: '/match/vesselsToOffer',
            vesselCost: '/match/updateVesselCost'
        }
    },

    Init: function () {
        this.OnProjectChange();
        this.OnSuitabilityChange();
        this.OnSubtypeChange();
        this.OnMatchClick();
        this.OnFilterOfferChange();
        Match.LoadOfferts();
        $('#btnMatch').click();
        $(window).resize(function () { Match.OnResize(); });
        $('#autopan').stick_in_parent();
        $('#vesselCard').stick_in_parent();
    },

    OnResize: function () {
        setTimeout(function () {
            var h = $("#sidebar").height() - 120;
            $("#lst-msg").css("min-height", h).css("height", h);
        }, 300);
    },

    OnSuitabilityChange: function () {
        this.vars.$suitability.on('change', this, this.LoadSubtype);
    },

    OnProjectChange: function () {
        this.vars.$project.on('change', this, this.OnLoadProject);
    },

    OnSubtypeChange: function () {
        this.vars.$subtype.on('change', this, this.OnLoadPulls)
    },

    OnMatchClick: function () {
        $('#btnMatch').on('click', this, this.FindMatch);
    },

    OnOffertVessel: function () {
        $('.btnOfertar').on('click', this, this.offertVessel);
    },

    OnFilterOfferChange: function () {
        this.vars.$offerFilter.on('change', this, this.LoadOfferts)
    },

    OnLoadProject: function (_this) {
        let ctx = _this.data;
        if ($(this).children("option:selected").val())
            ctx.Ajax(ctx.vars.url.project + "/" + $(this).children("option:selected").val(), 'GET', null, true, function (data, ctx) {
                if (data != null && data.Status == 0) {
                    ctx.FillSearch(data.Data);
                    ctx.EnableDisableControls(true);
                }
                else
                    ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.erro_load_project);
            });
        else
            ctx.EnableDisableControls(false);
    },

    OnLoadPulls: function (_this) {
        let ctx = _this.data;
        if ($(this).children("option:selected").val() == 1) {
            ctx.vars.$b_pull.removeClass("invisible2");
            ctx.vars.$b_pull_ahead.addClass("invisible2");
            ctx.vars.$b_pull_astern.addClass("invisible2");
        }
        if ($(this).children("option:selected").val() == 2) {
            ctx.vars.$b_pull.addClass("invisible2");
            ctx.vars.$b_pull_ahead.removeClass("invisible2");
            ctx.vars.$b_pull_astern.removeClass("invisible2");
        }
    },

    LoadSubtype: function (_this) {
        let ctx = _this.data;
        if ($(this).children("option:selected").val() == 1 || $(this).children("option:selected").val() == 2)
            ctx.vars.$subtype.removeClass("invisible2");
        else
            ctx.vars.$subtype.addClass("invisible2");
    },

    FillSearch: function (data) {
        this.vars.$suitability.val(data.ProjectTypeId);
        this.vars.$pemex.prop("checked", data.PemexCheck);
        this.vars.$region.val(data.RegionId);
        this.vars.$fStart.val(moment(data.StartDate).format("YYYY-MM-DD"));
        this.vars.$fEnd.val(moment(moment(data.StartDate)).add(data.Duration, 'days').format("YYYY-MM-DD"));
        this.vars.$cost.val(data.MaxRateBudget);
        if (data.DynamicPositionSystem - 1 > 0)
            $(this.vars.$systemPos[data.DynamicPositionSystem - 1]).prop("checked", true);
        else
            $.each($(this.vars.$systemPos), function (k, v) {
                $(v).prop("checked", false);
            });
        this.vars.$pasajeros.val(data.NumberPassenger);
        this.vars.$carga.val(data.CargoCapacity);
        if (this.vars.$suitability.val() == 1 || this.vars.$suitability.val() == 2) {
            this.vars.$subtype.removeClass("invisible2");
            this.vars.$subtype.val(data.SubtypeId);
            this.vars.$subtype.trigger('change');
        }
        this.vars.$b_pull.val(data.BollardPull);
        this.vars.$b_pull_ahead.val(data.BollardPullAhead);
        this.vars.$b_pull_astern.val(data.BollardPullAstern);
    },

    EnableDisableControls: function (state) {
        this.vars.$suitability.prop('disabled', state);
        this.vars.$pemex.prop('disabled', state);
        this.vars.$region.prop('disabled', state);
        this.vars.$fStart.prop('disabled', state);
        this.vars.$fEnd.prop('disabled', state);
        this.vars.$cost.prop('disabled', state);
        this.vars.$pasajeros.prop('disabled', state);
        this.vars.$carga.prop('disabled', state);
        $.each($(this.vars.$systemPos), function (k, v) {
            $(v).prop('disabled', state);
        })
        this.vars.$subtype.prop('disabled', state);

        this.vars.$b_pull.prop('disabled', state);
        this.vars.$b_pull_ahead.prop('disabled', state);
        this.vars.$b_pull_astern.prop('disabled', state);
    },

    FindMatch: function (_this) {
        let ctx = _this.data;
        let matchable = {
            SuitabilityId: ctx.vars.$suitability.children("option:selected").val(),
            PemexCheck: ctx.vars.$pemex.is(':checked') ? true : null,
            RegionId: ctx.vars.$region.children("option:selected").val(),
            StartDate: ctx.vars.$fStart.val(),
            EndDate: ctx.vars.$fEnd.val(),
            DailyMaxRate: ctx.vars.$cost.val(),
            DynamicPositionSystem: ($("input[name='postionSystem']:checked").val()) ? $("input[name='postionSystem']:checked").val() : null,
            NumberPassengers: ctx.vars.$pasajeros.val(),
            CargoCapacity: ctx.vars.$carga.val()
        }

        ctx.Ajax(ctx.vars.url.find, 'GET', matchable, true, function (data, ctx) {
            if (data != null && data.length > 0) {
                MapEgulf.RenderVesselsMarch(data, ctx.vars.$project.children("option:selected").val());
            }
            else {
                MapEgulf.RemoveVesselsMarker();
                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.erro_load_project);
            }
        });

    },

    LoadOfferts: function (_this) {
        //let Match = _this.data;
        Match.BlockUI("#sidebar");
        let f = $(this).children("option:selected").val() ? $(this).children("option:selected").val() : 0;
        Match.Ajax(Match.vars.url.offers + "/" + ($(this).children("option:selected").val() ? $(this).children("option:selected").val() : 0), 'GET', null, true, function (data, ctx) {
            $('#offersCard').empty();
            if (data != null && data.length > 0) {
                Match.vars.offers = data;
                $.each(data, function (key, value) {
                    Match.MapOffer(value);
                });
            }
            //else
            //Match.Toast("warning", "top rigth", Match.resources.txt_aviso, data.Message ? data.Message : Match.resources.erro_load_project);

            Match.UnblockUI("#sidebar");
        });
    },

    MapOffer: function (obj) {
        let isProject = obj.Type == 2;

        let template = `<div class="card-box widget-user col-md-12" style="padding-left: 10px; padding-bottom: 0px; margin-bottom: 5px;">
            <div class="row">
                <div class="col-sm-3" style="text-align: center;">
                    <img id="offerObjectIcon" src="${isProject ? `/Content/Images/ProjectCircle.png` : obj.Status == 2 && obj.Vessel.Image.Path ? obj.Vessel.Image.Path : `/Content/Images/BarcoCircle.png`}" class ="img-responsive rounded-circle" alt="user" style="padding-left: 10px;">
                    <p class="text-muted text-ligth-card" id="txtOwner">{owner}</p>
                </div>

                <div class="col-sm-8">
                    <h5 class="mt-0 m-b-8 font-16 text-primary"><b><span id="txtType">{vessel_type}</span> <span id="txtIMO" class="pull-right text-muted">{imo}</span></b> </h5>
                    <h5 class="m-t-15">
                        <img src="/Content/Images/calendar-tool-variant-for-time-administration.png" style="height:19px" />
                        <small class="text-muted m-l-5 text-ligth-card" id="txt-date">{date}</small>

                        <img src="/Content/Images/delete.png" class ="float-right m-l-5" id="icoStatusRej{offerId}" style="height:19px; display: ${obj.Status === 3 || obj.Status === 4 ? `block` : `none`}  " />
                        <img src="/Content/Images/Palomita.png" class ="float-right m-l-5" id="icoStatusAcept{offerId}" style="height:19px; display: ${obj.Status === 2 ? `block` : `none`}  " />
                        ${obj.OfferReceived ? `<img src="/Content/Images/arrows-green.png" style="height:19px" class ="float-right m-l-5" id="icoDir"/>` : `<img src="/Content/Images/arrows-red.png" style="height:19px" class ="float-right m-l-5" id="icoDir{offerId}"/>`}
                    </h5>
                    <h5 class="m-t-15">
                        <img src="/Content/Images/placeholder.png" style="height:21px" />
                        <small class="text-muted m-l-5 text-ligth-card" id="txtPort">{port}</small>
                        ${!isProject ? `<small class="text-muted m-l-5 float-right text-ligth-card" style="cursor:pointer;" data-toggle="collapse" data-target="#detail_offer_vessel_${obj.OfferId}" id="viewDetail">Ver detalle<i class="fa fa-caret-down"></i></small>` : `<small class="text-muted m-l-5 float-right text-ligth-card" style="cursor:pointer;" data-toggle="collapse" data-target="#detail_offer_project_${obj.OfferId}" id="viewDetail">Ver detalle<i class="fa fa-caret-down"></i></small>`}
                    </h5>
                </div>
                <div id="btnGroup{offerId}" class="col-sm-1  pl-1 border border-right-0 border-top-0 border-bottom-0 m-b-15">
                    ${obj.Type === 2 && (obj.Status === 1 || obj.Status === 5) ?
                `<button class="badge waves-effect waves-light btn-success m-b-5 p-r-3" id="btnAcceptOffer" data-OfferId="{offerId}"> <i class="fa fa-check" onclick="MapEgulf.ValTransaction('ACCEPT', {offerId},{OfferReceived})"></i> </button>
                    <button class ="badge waves-effect waves-light btn-danger m-b-5"> <i class ="fa fa-remove" id="btnCancelOffer" data-OfferId="{offerId}" onclick="MapEgulf.ValTransaction('REJECT', {offerId})"></i> </button>` : ``}
                    <button style="display: ${obj.Status === 5 || (obj.Type === 2 && obj.Status === 1) ? `block` : `none`}" class ="badge waves-effect waves-light btn-info m-b-5 ${obj.MessageNotReaded > 0 ? `btn-warning` : ``}"> <i class ="fa fa-comments-o" data-status={status} id="btnChatOffer" data-OfferId="{offerId}" onclick="MapEgulf.ValTransaction('FIX', {offerId})" ></i> </button>
                </div>
                ${!isProject ?
                `<div id="detail_offer_vessel_${obj.OfferId}" class="collapse col-sm-12 title-card">
                    <hr />
                    <div id="vesselInfo">
                        ${obj.Status == 2 ? !isProject ?
                    `<div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Nombre Barco</h5>
                                <p class ="text-ligth-card text-muted">{vesselName}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">IMO</h5>
                                <p class ="text-ligth-card text-muted">{IMO}</p>
                            </div>
                        < /div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Tonelaje Bruto</h5>
                                <p class ="text-ligth-card text-muted">{grossTonnage}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Calado a Carga Máxima</h5>
                                <p class ="text-ligth-card text-muted">{maxLoadedDraft}</p>
                            </div>
                        < /div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Manga Total</h5>
                                <p class ="text-ligth-card text-muted">{beamOverall}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Eslora Total</h5>
                                <p class ="text-ligth-card text-muted">{lengthOverall}</p>
                            </div>
                        < /div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Recuperación de Hidrocarburos</h5>
                                <p class ="text-ligth-card text-muted">{oilRecoveryCapacity}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Planta de Fabricación de Agua</h5>
                                <p class ="text-ligth-card text-muted">{waterMakerPlant}</p>
                            </div>
                        < /div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Procesadora de Agua Caliente</h5>
                                <p class ="text-ligth-card text-muted">{hotWaterCalorifier}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Plata de Tratamiento de Aguas Residuales</h5>
                                <p class ="text-ligth-card text-muted">{sewageTreatmentPlant}</p>
                            </div>
                        < /div>`
                    : ``

                    : ``}
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Año Construcción</h5>
                                <p class ="text-ligth-card text-muted">{yearBuild}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Clase</h5>
                                <p class ="text-ligth-card text-muted">{classSociety}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Notacion de Clase</h5>
                                <p class ="text-ligth-card text-muted">{classNotation}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Validad de Clase</h5>
                                <p class ="text-ligth-card text-muted">{classValidity}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Adecuabilidad</h5>
                                <p class ="text-ligth-card text-muted">{suitability}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Región</h5>
                                <p class ="text-ligth-card text-muted">{region}</p>
                            </div>
                        </div>
                    </div>
                    <div id="specificInfo">
                    <hr />
                        <div class="row">
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Sistema de navegación</h5>
                                <p class="text-ligth-card text-muted" id="txtNavegacion">{navigation}</p>
                            </div>
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">BHP</h5>
                                <p class="text-ligth-card text-muted" id="txtBHP">{BHP}</p>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Subcategoría</h5>
                                <p class="text-ligth-card text-muted" id="txtSubCat">{subCat}</p>
                            </div>
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Tirón a Punto Fijo</h5>
                                <p class="text-ligth-card text-muted" id="txtBP">{BP}</p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Tirón a Proa</h5>
                                <p class="text-ligth-card text-muted" id="txtBPAh">{BPAh}</p>
                            </div>
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Tirón a Popa</h5>
                                <p class="text-ligth-card text-muted" id="txtBPAs">{BPAs}</p>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Número de pasajeros</h5>
                                <p class="text-ligth-card text-muted" id="txtPasajeros">{Pasajeros}</p>
                            </div>
                            <div class="col-sm-6">
                                <h5 class="font-14 font-weight-bold text-muted">Capacidad de Carga</h5>
                                <p class="text-ligth-card text-muted" id="txtCarga">{Carga}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Tonelaje Neto</h5>
                                <p class ="text-ligth-card text-muted">{netTonnage}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Manga Total</h5>
                                <p class ="text-ligth-card text-muted">{BeamOverall}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Eslora Total</h5>
                                <p class ="text-ligth-card text-muted">{lengtOverall}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Calado a Carga Máxima</h5>
                                <p class ="text-ligth-card text-muted">{maxLoadedDraft}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Área Libre de Cubierta</h5>
                                <p class ="text-ligth-card text-muted">{freeDeckArea}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Resistencia de Cubierta</h5>
                                <p class ="text-ligth-card text-muted">{deckStregnght}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Agua Potable</h5>
                                <p class ="text-ligth-card text-muted">{freshWater}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Combustible</h5>
                                <p class ="text-ligth-card text-muted">{fuelOil}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Agua de Lastro</h5>
                                <p class ="text-ligth-card text-muted">{ballastWater}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Tanque de Lodo</h5>
                                <p class ="text-ligth-card text-muted">{mudCapacity}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Tanque Cemento</h5>
                                <p class ="text-ligth-card text-muted">{cementTanks}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas</h5>
                                <p class ="text-ligth-card text-muted">{cabinQuantity}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Velocidad de Crucero</h5>
                                <p class ="text-ligth-card text-muted">{cruisingSpeed}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Máxima Velocidad</h5>
                                <p class ="text-ligth-card text-muted">{maxSpeed}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Autonomía Velocidad Económica</h5>
                                <p class ="text-ligth-card text-muted">{distanceRangeCruisingSpeed}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Autonomía Velocidad Máxima</h5>
                                <p class ="text-ligth-card text-muted">{distanceRangeMaximumSpeed}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Consumo de Combustible Velocidad Económica</h5>
                                <p class ="text-ligth-card text-muted">{fuelConsumptionCruisingSpeed}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Consumo de Combustible Máxima Velocidad</h5>
                                <p class ="text-ligth-card text-muted">{fuelConsumptionMaximumSpeed}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas Individuales</h5>
                                <p class ="text-ligth-card text-muted">{singleBerth}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas Dobles</h5>
                                <p class ="text-ligth-card text-muted">{doubleBerth}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas Cuádruples</h5>
                                <p class ="text-ligth-card text-muted">{fourBerth}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas con A/C</h5>
                                <p class ="text-ligth-card text-muted">{airConditioning}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Comedor</h5>
                                <p class ="text-ligth-card text-muted">{messRoom}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Sala de Control</h5>
                                <p class ="text-ligth-card text-muted">{controlRoom}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Sala de Juntas</h5>
                                <p class ="text-ligth-card text-muted">{conferenceRoom}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Gimnasio</h5>
                                <p class ="text-ligth-card text-muted">{gymnasium}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Piscina</h5>
                                <p class ="text-ligth-card text-muted">{swimingPool}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Oficinas</h5>
                                <p class ="text-ligth-card text-muted">{offices}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Hospital</h5>
                                <p class ="text-ligth-card text-muted">{hospital}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad de Descarga</h5>
                                <p class ="text-ligth-card text-muted">{pumpRates}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Disponibilidad</h5>
                                <p class ="text-ligth-card text-muted">{vesselAvailability}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Check-List Pemex</h5>
                                <p class ="text-ligth-card text-muted">{pemex}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Costo de Acomodación</h5>
                                <p class ="text-ligth-card text-muted">{lodgingCost}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Costo de Alimentación</h5>
                                <p class ="text-ligth-card text-muted">{mealsCost}</p>
                            </div>
                        </div>

                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Costo Lavandería</h5>
                                <p class ="text-ligth-card text-muted">{laundryCost}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Tarifa Díaria</h5>
                                <p class ="text-ligth-card text-muted">{dailyRate}</p>
                            </div>
                        </div>

                    </div>
                </div>`
                : `<div id="detail_offer_project_${obj.OfferId}" class="collapse col-sm-12 title-card">
                    <hr />
                    <div id="ProjectInfo">
                         <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Region</h5>
                                <p class ="text-ligth-card text-muted">{region}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Extensión</h5>
                                <p class ="text-ligth-card text-muted">{extension}</p>
                            </div>
                             <!--<div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Presupuesto</h5>
                                <p class ="text-ligth-card text-muted">{BudgetMaxRate}</p>
                                </div>-->
                         </div>
                         <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Área Libre de Cubierta</h5>
                                <p class ="text-ligth-card text-muted">{freeDeckArea}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Tanque de Lodo</h5>
                                <p class ="text-ligth-card text-muted">{mudCapacity}</p>
                            </div>
                         </div>
                         <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Tanque Cemento</h5>
                                <p class ="text-ligth-card text-muted">{cementTankCapacity}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad Recuperación de Hidrocarburos</h5>
                                <p class ="text-ligth-card text-muted">{oilRecoveryCapacity}</p>
                            </div>
                         </div>
                         <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Sistema de Posicionamiento Dinámico</h5>
                                <p class ="text-ligth-card text-muted">{dynamicPositionSystem}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Check-List de Pemex</h5>
                                <p class ="text-ligth-card text-muted">{pemexCheck}</p>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div id="ProjectSpecificInfo">
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Subcategoría</h5>
                                <p class ="text-ligth-card text-muted">{subtype}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Tiron a Punto Fijo</h5>
                                <p class ="text-ligth-card text-muted">{bollardPull}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Tirón a Proa</h5>
                                <p class ="text-ligth-card text-muted">{bollardPullAhead}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Tirón a Popa</h5>
                                <p class ="text-ligth-card text-muted">{bollardPullAstern}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Número de Pasajeros</h5>
                                <p class ="text-ligth-card text-muted">{numberPassenger}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas Individuales</h5>
                                <p class ="text-ligth-card text-muted">{singleBerth}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas Dobles</h5>
                                <p class ="text-ligth-card text-muted">{doubleBerth}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas Cuádruples</h5>
                                <p class ="text-ligth-card text-muted">{fourBerth}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Cabinas con A/C</h5>
                                <p class ="text-ligth-card text-muted">{airCondition}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Comedor</h5>
                                <p class ="text-ligth-card text-muted">{messRoom}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Sala de Control</h5>
                                <p class ="text-ligth-card text-muted">{controlRoom}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Sala de Juntas</h5>
                                <p class ="text-ligth-card text-muted">{conferenceRoom}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Gimnasio</h5>
                                <p class ="text-ligth-card text-muted">{gymnasium}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Piscina</h5>
                                <p class ="text-ligth-card text-muted">{swimingPool}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Oficinas</h5>
                                <p class ="text-ligth-card text-muted">{office}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Hospital</h5>
                                <p class ="text-ligth-card text-muted">{hospital}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad de Carga en Cubierta</h5>
                                <p class ="text-ligth-card text-muted">{cargoCapacity}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Resistencia en Cubierta</h5>
                                <p class ="text-ligth-card text-muted">{deckStrenght}</p>
                            </div>
                        </div>
                        <div class ="row">
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad de Tanques</h5>
                                <p class ="text-ligth-card text-muted">{tankCapacity}</p>
                            </div>
                            <div class ="col-sm-6">
                                <h5 class ="font-14 font-weight-bold text-muted">Capacidad de Bombeo</h5>
                                <p class ="text-ligth-card text-muted">{dischargeRate}</p>
                            </div>
                        </div>
                    </div>
                   ${obj.Status == 2 ? `` : ``}
                </div>`}
                </div>
        </div>`;
        let res;
        if (!isProject) {
            res =
                template.replace('{owner}', obj.VesselAdmin.FirstName ? obj.VesselAdmin.FirstName : "")
                    .replace('{status}', obj.Status)
                    .replace('{vessel_type}', obj.Vessel.VesselType.Name.length > 10 ? obj.Vessel.VesselType.Name.substring(0, 15) + "..." : obj.Vessel.VesselType.Name)
                    .replace('{imo}', obj.Vessel.Country.Name ? obj.Vessel.Country.Name : "-")
                    .replace('{date}', obj.OfferedDate ? moment(obj.OfferedDate).format("YYYY/MMM/DD") : "-")
                    .replace('{port}', obj.Vessel.HomePort.Name ? obj.Vessel.HomePort.Name : "-")
                    .replace('{yearBuild}', obj.Vessel.YearBuild ? obj.Vessel.YearBuild : "-")
                    .replace('{classSociety}', obj.Vessel.ClasificationSociety ? obj.Vessel.ClasificationSociety.Name : "-")
                    .replace('{classNotation}', obj.Vessel.ClassNotation ? obj.Vessel.ClassNotation : "-")
                    .replace('{classValidity}', obj.Vessel.ClassValidity ? moment(obj.Vessel.ClassValidity).format("YYYY/MMM/DD") : "-")
                    .replace('{suitability}', obj.Vessel.SuitabilityDescription ? obj.Vessel.SuitabilityDescription : "-")
                    .replace('{BHP}', obj.VesselSpecificInfoModelExtra.BHP ? obj.VesselSpecificInfoModelExtra.BHP : "-")
                    .replace('{subCat}', obj.VesselSpecificInfoModelExtra.Subtype ? obj.VesselSpecificInfoModelExtra.Subtype : "-")
                    .replace('{BP}', obj.VesselSpecificInfoModelExtra.BollardPull ? obj.VesselSpecificInfoModelExtra.BollardPull : "-")
                    .replace('{BPAh}', obj.VesselSpecificInfoModelExtra.BollardPullAhead ? obj.VesselSpecificInfoModelExtra.BollardPullAhead : "-")
                    .replace('{BPAs}', obj.VesselSpecificInfoModelExtra.BollardPullAstern ? obj.VesselSpecificInfoModelExtra.BollardPullAstern : "-")
                    .replace('{Carga}', obj.VesselSpecificInfoModelExtra.CargoCapacity ? obj.VesselSpecificInfoModelExtra.CargoCapacity : "-")
                    .replace('{netTonnage}', obj.VesselSpecificInfoModel.NetTonnage ? obj.VesselSpecificInfoModel.NetTonnage : "-")
                    .replace('{BeamOverall}', obj.VesselSpecificInfoModel.BeamOverall ? obj.VesselSpecificInfoModel.BeamOverall : "-")
                    .replace('{lengtOverall}', obj.VesselSpecificInfoModel.LengthOverall ? obj.VesselSpecificInfoModel.LengthOverall : "-")
                    .replace('{maxLoadedDraft}', obj.VesselSpecificInfoModel.MaximumLoadedDraft ? obj.VesselSpecificInfoModel.MaximumLoadedDraft : "-")
                    .replace('{freeDeckArea}', obj.VesselSpecificInfoModel.freeDeckArea ? obj.VesselSpecificInfoModel.freeDeckArea : "-")
                    .replace('{deckStregnght}', obj.VesselSpecificInfoModel.DeckStrenght ? obj.VesselSpecificInfoModel.DeckStrenght : "-")
                    .replace('{freshWater}', obj.VesselSpecificInfoModel.FreshWaterCapacity ? obj.VesselSpecificInfoModel.FreshWaterCapacity : "-")
                    .replace('{fuelOil}', obj.VesselSpecificInfoModel.FuelOilCapacity ? obj.VesselSpecificInfoModel.FuelOilCapacity : "-")
                    .replace('{ballastWater}', obj.VesselSpecificInfoModel.BallastWaterCapacity ? obj.VesselSpecificInfoModel.BallastWaterCapacity : "-")
                    .replace('{mudCapacity}', obj.VesselSpecificInfoModel.MudCapacity ? obj.VesselSpecificInfoModel.MudCapacity : "-")
                    .replace('{cementTanks}', obj.VesselSpecificInfoModel.CementTanksCapacity ? obj.VesselSpecificInfoModel.CementTanksCapacity : "-")
                    .replace('{cruisingSpeed}', obj.VesselSpecificInfoModel.CruisingSpeed ? obj.VesselSpecificInfoModel.CruisingSpeed : "-")
                    .replace('{maxSpeed}', obj.VesselSpecificInfoModel.MaximumSpeed ? obj.VesselSpecificInfoModel.MaximumSpeed : "-")
                    .replace('{distanceRangeCruisingSpeed}', obj.VesselSpecificInfoModel.DistanceCruisingSpeed ? obj.VesselSpecificInfoModel.DistanceCruisingSpeed : "-")
                    .replace('{distanceRangeMaximumSpeed}', obj.VesselSpecificInfoModel.DistanceMaxSpeed ? obj.VesselSpecificInfoModel.DistanceMaxSpeed : "-")
                    .replace('{fuelConsumptionCruisingSpeed}', obj.VesselSpecificInfoModel.FuelConsumptionCruisingSpeed ? obj.VesselSpecificInfoModel.FuelConsumptionCruisingSpeed : "-")
                    .replace('{fuelConsumptionMaximumSpeed}', obj.VesselSpecificInfoModel.FuelConsumptionMaxSpeed ? obj.VesselSpecificInfoModel.FuelConsumptionMaxSpeed : "-")
                    .replace('{cabinQuantity}', obj.VesselSpecificInfoModelExtra.CabinQuantity ? obj.VesselSpecificInfoModelExtra.CabinQuantity : "-")
                    .replace('{singleBerth}', obj.VesselSpecificInfoModelExtra.SingleBerth ? obj.VesselSpecificInfoModelExtra.SingleBerth : "-")
                    .replace('{doubleBerth}', obj.VesselSpecificInfoModelExtra.DoubleBerth ? obj.VesselSpecificInfoModelExtra.DoubleBerth : "-")
                    .replace('{fourBerth}', obj.VesselSpecificInfoModelExtra.FourBerth ? obj.VesselSpecificInfoModelExtra.FourBerth : "-")
                    .replace('{airConditioning}', obj.VesselSpecificInfoModelExtra.AirCondition ? "Si" : "No")
                    .replace('{messRoom}', obj.VesselSpecificInfoModelExtra.MessRoom ? obj.VesselSpecificInfoModelExtra.MessRoom : "-")
                    .replace('{controlRoom}', obj.VesselSpecificInfoModelExtra.ControlRoom ? "Si" : "No")
                    .replace('{conferenceRoom}', obj.VesselSpecificInfoModelExtra.ConferenceRoom ? "Si" : "No")
                    .replace('{gymnasium}', obj.VesselSpecificInfoModelExtra.Gymnasium ? "Si" : "No")
                    .replace('{swimingPool}', obj.VesselSpecificInfoModelExtra.SwimingPool ? "Si" : "No")
                    .replace('{offices}', obj.VesselSpecificInfoModelExtra.Office ? "Si" : "No")
                    .replace('{hospital}', obj.VesselSpecificInfoModelExtra.Hospital ? "Si" : "No")
                    .replace('{pumpRates}', obj.VesselSpecificInfoModelExtra.PumpRates ? obj.VesselSpecificInfoModelExtra.PumpRates : "-")
                    //.replace('{vesselAvailability}', obj.VesselSpecificInfoModelExtra.HospVital ? obj.VesselSpecificInfoModelExtra.Hospital : "-")
                    .replace('{pemex}', obj.VesselSpecificInfoModelExtra.PemexCheck ? "Si" : "No")
                    .replace('{lodgingCost}', obj.Vessel.VesselCost.LodgingCost ? obj.Vessel.VesselCost.LodgingCost : "-")
                    .replace('{mealsCost}', obj.Vessel.VesselCost.MealCost ? obj.Vessel.VesselCost.MealCost : "-")
                    .replace('{laundryCost}', obj.Vessel.VesselCost.LaundryCost ? obj.Vessel.VesselCost.LaundryCost : "-")
                    .replace('{dailyRate}', obj.Vessel.VesselCost.DailyRateFlotel ? obj.Vessel.VesselCost.DailyRateFlotel : "-")
                    .replace('{vesselName}', obj.Vessel.Name ? obj.Vessel.Name : "-")
                    .replace('{IMO}', obj.Vessel.Imo ? obj.Vessel.Imo : "-")
                    .replace('{grossTonnage}', obj.VesselSpecificInfoModel.GrossTonnage ? obj.VesselSpecificInfoModel.GrossTonnage : "-")
                    .replace('{maxLoadedDraft}', obj.VesselSpecificInfoModel.MaximumLoadedDraft ? obj.VesselSpecificInfoModel.MaximumLoadedDraft : "-")
                    .replace('{beamOverall}', obj.VesselSpecificInfoModel.BeamOverall ? obj.VesselSpecificInfoModel.BeamOverall : "-")
                    .replace('{lengthOverall}', obj.VesselSpecificInfoModel.LengthOverall ? obj.VesselSpecificInfoModel.LengthOverall : "-")
                    .replace('{oilRecoveryCapacity}', obj.VesselSpecificInfoModel.OilRecoveryCapacity ? obj.VesselSpecificInfoModel.OilRecoveryCapacity : "-")
                    .replace('{waterMakerPlant}', obj.VesselSpecificInfoModel.WaterMarkerPlant ? obj.VesselSpecificInfoModel.WaterMarkerPlant : "-")
                    .replace('{hotWaterCalorifier}', obj.VesselSpecificInfoModel.HotWaterCalorifier ? obj.VesselSpecificInfoModel.HotWaterCalorifier : "-")
                    .replace('{sewageTreatmentPlant}', obj.VesselSpecificInfoModel.SewageTreatmentPlant ? obj.VesselSpecificInfoModel.SewageTreatmentPlant : "-")

                    .replace(/{offerId}/g, obj.OfferId);


        }
        else {
            res =
                template.replace(/{owner}/g, obj.ProjectAdmin.FirstName)
                    .replace('{status}', obj.Status)
                    .replace(/{vessel_type}/g, obj.Project.Folio)
                    .replace(/{imo}/g, obj.Project.ProjectType)
                    .replace(/{date}/g, moment(obj.Project.StartDate).format("DD MMM") + " - " + moment(moment(obj.Project.StartDate)).add(obj.Project.Duration, 'days').format("DD MMM"))
                    .replace(/{port}/g, obj.Project.CompanyName)
                    .replace(/{offerId}/g, obj.OfferId)
                    .replace(/{region}/g, obj.Project.Region ? obj.Project.Region : "-")
                    .replace(/{extension}/g, obj.Project.Extension ? "Sí" : "No")
                    .replace(/{freeDeckArea}/g, obj.Project.FreeDeckArea ? obj.Project.FreeDeckArea : "-")
                    .replace(/{mudCapacity}/g, obj.Project.MudCapacity ? obj.Project.MudCapacity : "-")
                    .replace(/{cementTankCapacity}/g, obj.Project.CementTankCapacity ? obj.Project.CementTankCapacity : "-")
                    .replace(/{oilRecoveryCapacity}/g, obj.Project.OilRecoveryCapacity ? obj.Project.OilRecoveryCapacity : "-")
                    .replace(/{dynamicPositionSystem}/g, obj.Project.DynamicPositionSystemName ? obj.Project.DynamicPositionSystemName : "-")
                    .replace(/{pemexCheck}/g, obj.Project.PemexCheck ? "Requerido" : "No requerido")
                    .replace(/{subtype}/g, obj.Project.Subtype ? obj.Project.Subtype : "-")
                    .replace(/{bollardPull}/g, obj.Project.BollardPull ? obj.Project.BollardPull : "-")
                    .replace(/{bollardPullAhead}/g, obj.Project.BollardPullAhead ? obj.Project.BollardPullAhead : "-")
                    .replace(/{bollardPullAstern}/g, obj.Project.BollardPullAstern ? obj.Project.BollardPullAstern : "-")
                    .replace(/{numberPassenger}/g, obj.Project.NumberPassenger ? obj.Project.NumberPassenger : "-")
                    .replace(/{singleBerth}/g, obj.Project.SingleBerth ? obj.Project.SingleBerth : "-")
                    .replace(/{doubleBerth}/g, obj.Project.DoubleBerth ? obj.Project.DoubleBerth : "-")
                    .replace(/{fourBerth}/g, obj.Project.FourBerth ? obj.Project.FourBerth : "-")
                    .replace(/{airCondition}/g, obj.Project.AirCondition ? "Sí" : "No")
                    .replace(/{messRoom}/g, obj.Project.MessRoom ? "Sí" : "No")
                    .replace(/{controlRoom}/g, obj.Project.ControlRoom ? "Sí" : "No")
                    .replace(/{conferenceRoom}/g, obj.Project.ConferenceRoom ? "Sí" : "No")
                    .replace(/{gymnasium}/g, obj.Project.Gymnasium ? "Sí" : "No")
                    .replace(/{swimingPool}/g, obj.Project.SwimingPool ? "Sí" : "No")
                    .replace(/{office}/g, obj.Project.Office ? "Sí" : "No")
                    .replace(/{hospital}/g, obj.Project.Hospital ? "Sí" : "No")
                    .replace(/{cargoCapacity}/g, obj.Project.CargoCapacity ? obj.Project.CargoCapacity : "-")
                    .replace(/{deckStrenght}/g, obj.Project.DeckStrenght ? obj.Project.DeckStrenght : "-")
                    .replace(/{tankCapacity}/g, obj.Project.TankCapacity ? obj.Project.TankCapacity : "-")
                    .replace(/{dischargeRate}/g, obj.Project.DischargeRate ? obj.Project.DischargeRate : "-")
                    .replace(/{OfferReceived}/g, obj.OfferReceived)
        }

        $('#offersCard').append(res);
    },

    MapVesselToOffer: function (model) {
        console.log(model);
        this.Ajax(this.vars.url.vesselsToOffer, 'GET', model, true, function (data, ctx) {
            if (data != null && data.length > 0) {
                console.log(data);
                $('#vesselCard').empty();
                $.each(data, function (k, v) {
                    Match._MapVesselToOffer(v, model.offer);
                })
                ctx.OnOffertVessel();
            }
            else {
                MapEgulf.RemoveVesselsMarker();
                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.erro_load_project);
            }
        });
    },

    _MapVesselToOffer: function (obj, offer) {
        let template = `<div class="card-box widget-user col-md-12" style="padding-left: 10px; padding-bottom: 0px; margin-bottom: 5px;">
            <div class="row">
                <div class="col-sm-3" style="text-align: center;">
                    <img id="offerObjectIcon" src="/Content/Images/BarcoCircle.png" class ="img-responsive rounded-circle" alt="user" style="padding-left: 10px;">
                    <p class="text-muted text-ligth-card" id="txtOwner">{owner}</p>
                </div>

                <div class="col-sm-8">
                    <h5 class="mt-0 m-b-8 font-16 text-primary"><b><span id="txtType">{vessel_type}</span> <span id="txtIMO" class="pull-right text-muted">{imo}</span></b> </h5>
                    <h5 class="m-t-15">
                        <img src="/Content/Images/placeholder.png" style="height:19px" />
                        <small class="text-muted m-l-5 text-ligth-card" id="txt-date">{date}</small>
                    </h5>
                    <h5 class="m-t-15">
                        <img src="/Content/Images/placeholder.png" style="height:21px" />
                        <small class ="text-muted m-l-5 text-ligth-card" id="txtPort">{port}</small>
                        <div class ="btn btn-sm btn-primary float-right"  data-toggle="collapse" data-target="#detail_offer_vessel_${obj.VesselId}">Asignar</div>
                    </h5>

                </div>
                <div id="detail_offer_vessel_${obj.VesselId}" class ="collapse col-sm-12 title-card">
                    <hr />
                    <div class ="row">
                        <div class ="form-group col-sm-12">
                            <div class ="input-group input-sm">
                                <span class ="bg-primary input-group-addon b-0 text-white">
                                    <i class ="fa fa-dollar"></i>
                                </span>
                                <input type="text" disabled class ="form-control input-sm text-form-contro-sm" id="costsA" placeholder="Costo de acomodación" value="${obj.VesselCost.DailyMaxRate ? obj.VesselCost.LodgingCost : ""}"/>
                            </div>
                        </div>
                    </div>
                    <div class ="row">
                        <div class ="form-group col-sm-12">
                            <div class ="input-group input-sm">
                                <span class ="bg-primary input-group-addon b-0 text-white">
                                    <i class ="fa fa-dollar"></i>
                                </span>
                                <input type="text" disabled class ="form-control input-sm text-form-contro-sm" id="costsAl" placeholder="Costo de Alimentación" value="${obj.VesselCost.MealCost ? obj.VesselCost.MealCost : ""}" />
                            </div>
                        </div>
                    </div>
                    <div class ="row">
                        <div class ="form-group col-sm-12">
                            <div class ="input-group input-sm">
                                <span class ="bg-primary input-group-addon b-0 text-white">
                                    <i class ="fa fa-dollar"></i>
                                </span>
                                <input type="text" disabled class ="form-control input-sm text-form-contro-sm" id="costsL" placeholder="Costo de Lavanderia" value="${obj.VesselCost.LaundryCost ? obj.VesselCost.LaundryCost : ""}" />
                            </div>
                        </div>
                    </div>
                    <div class ="row">
                        <div class ="form-group col-sm-12">
                            <div class ="input-group input-sm">
                                <span class ="bg-primary input-group-addon b-0 text-white">
                                    <i class ="fa fa-dollar"></i>
                                </span>
                                <input type="text" class ="form-control input-sm text-form-contro-sm costsD" placeholder="Costo Diario" value="${obj.VesselCost.DailyMaxRate ? obj.VesselCost.DailyMaxRate : ""}" />
                            </div>
                        </div>
                    </div>
                    <button type="button" class ="btn btn-sm btn-primary float-right btnOfertar" data-offer="${offer}"  data-index="${obj.VesselId}">Ofertar</button>
                </div>`;

        let res = template.replace('{owner}', obj.Name ? obj.Name : "")
            .replace('{vessel_type}', obj.VesselType.Name.length > 10 ? obj.VesselType.Name.substring(0, 15) + "..." : obj.VesselType.Name)
            .replace('{imo}', obj.Imo ? obj.Imo : "-")
            .replace('{date}', obj.HomePort.Region.Name ? obj.HomePort.Region.Name : "-")
            .replace('{port}', obj.HomePort.Name ? obj.HomePort.Name : "-")

        $('#vesselCard').append(res);
    },

    offertVessel: function (_this) {
        let ctx = _this.data;
        let model = {
            OfferId: $(this).data('offer'),
            DailyRate: $(this).prev('div.row').find('.costsD').val()
        };
        ctx.Ajax(ctx.vars.url.vesselCost, 'GET', model, true, function (data, ctx) {
            if (data.Status == 0) {
                MapEgulf.ValTransaction("ACCEPT", model.OfferId);
                $('#vesselCard').empty();
            }
            else {

                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.erro_load_project);
            }
        });
    }

}