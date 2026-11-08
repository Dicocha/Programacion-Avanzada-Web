(function () {
    "use strict";

    var intervaloActualizacionTickets = 5000;
    var actualizandoTickets = false;
    var temporizadorTickets = null;
    var filtrosTickets = null;

    $(function () {
        activarNavegacion();
        inicializarFiltroTickets();
        inicializarActualizacionTickets();
        inicializarComentarios();
        inicializarGestionTicket();
    });

    function activarNavegacion() {
        var path = window.location.pathname.toLowerCase();
        var activeLink = null;
        var activeLength = 0;

        $(".navbar-nav .nav-link").each(function () {
            var href = ($(this).attr("href") || "").toLowerCase();

            if (href && href !== "/" && path.indexOf(href) === 0 && href.length > activeLength) {
                activeLink = this;
                activeLength = href.length;
            }
        });

        if (activeLink) {
            $(activeLink).addClass("active");
        }
    }

    function inicializarFiltroTickets() {
        var $form = $("#ticketsFiltro");
        if (!$form.length) {
            return;
        }

        $form.on("submit", function (event) {
            event.preventDefault();

            if (actualizandoTickets) {
                return;
            }

            var $contenedor = $("#ticketsTableContainer");
            var $boton = $form.find("button[type='submit']");

            filtrosTickets = $form.serialize();
            clearTimeout(temporizadorTickets);
            actualizandoTickets = true;
            $boton.prop("disabled", true);
            $contenedor.css("opacity", 0.5);

            $.ajax({
                url: $form.attr("action"),
                type: "GET",
                data: filtrosTickets,
                dataType: "html",
                cache: false
            }).done(function (html) {
                if (esPantallaLogin(html)) {
                    window.location.reload();
                    return;
                }

                $contenedor.html(html);
            }).fail(function () {
                $contenedor.html('<div class="alert alert-danger">No se pudo aplicar el filtro. Intentá nuevamente.</div>');
            }).always(function () {
                actualizandoTickets = false;
                $boton.prop("disabled", false);
                $contenedor.css("opacity", 1);
                programarActualizacionTickets($form, $contenedor);
            });
        });
    }

    function inicializarActualizacionTickets() {
        var $form = $("#ticketsFiltro");
        var $contenedor = $("#ticketsTableContainer");

        if (!$form.length || !$contenedor.length) {
            return;
        }

        filtrosTickets = $form.serialize();

        $(document).on("visibilitychange", function () {
            if (!document.hidden) {
                clearTimeout(temporizadorTickets);
                actualizarTicketsSilencioso($form, $contenedor);
            }
        });

        programarActualizacionTickets($form, $contenedor);
    }

    function programarActualizacionTickets($form, $contenedor) {
        clearTimeout(temporizadorTickets);
        temporizadorTickets = setTimeout(function () {
            actualizarTicketsSilencioso($form, $contenedor);
        }, intervaloActualizacionTickets);
    }

    function actualizarTicketsSilencioso($form, $contenedor) {
        if (document.hidden || actualizandoTickets) {
            programarActualizacionTickets($form, $contenedor);
            return;
        }

        actualizandoTickets = true;

        $.ajax({
            url: $form.attr("action"),
            type: "GET",
            data: filtrosTickets,
            dataType: "html",
            cache: false,
            timeout: 10000
        }).done(function (html) {
            if (esPantallaLogin(html)) {
                window.location.reload();
                return;
            }

            if (html !== $contenedor.html()) {
                $contenedor.html(html);
            }
        }).always(function () {
            actualizandoTickets = false;
            programarActualizacionTickets($form, $contenedor);
        });
    }

    function esPantallaLogin(html) {
        return typeof html === "string" && html.indexOf('name="Identificador"') !== -1;
    }

    function inicializarComentarios() {
        var $form = $("#comentarioForm");
        if (!$form.length) {
            return;
        }

        $form.on("submit", function (event) {
            event.preventDefault();

            if ($form.data("validator") && !$form.valid()) {
                return;
            }

            var $boton = $("#comentarioSubmit");
            var $feedback = $("#comentarioFeedback");

            $boton.prop("disabled", true).text("Enviando...");

            $.ajax({
                url: $form.attr("action"),
                type: "POST",
                data: $form.serialize(),
                dataType: "json"
            }).done(function (respuesta) {
                if (!respuesta || !respuesta.ok) {
                    mostrarFeedback($feedback, (respuesta && respuesta.message) || "No se pudo agregar el comentario.", "danger");
                    return;
                }

                $("#comentariosLista").removeClass("d-none").append(crearComentario(respuesta.comentario));
                $("#comentariosVacio").addClass("d-none");
                $("#comentarioTexto").val("");
                $("#fechaActualizacionTexto").text(respuesta.fechaActualizacion);
                mostrarFeedback($feedback, "Comentario agregado.", "success");
                ocultarFeedback($feedback, 3000);
            }).fail(function () {
                mostrarFeedback($feedback, "No se pudo agregar el comentario. Intentá nuevamente.", "danger");
            }).always(function () {
                $boton.prop("disabled", false).text("Comentar");
            });
        });
    }

    function inicializarGestionTicket() {
        $("#asignacionForm").on("submit", function (event) {
            event.preventDefault();
            enviarGestionTicket($(this), "#asignacionFeedback");
        });

        $("#estadoForm").on("submit", function (event) {
            event.preventDefault();
            enviarGestionTicket($(this), "#estadoFeedback");
        });
    }

    function enviarGestionTicket($form, feedbackSelector) {
        var $feedback = $(feedbackSelector);
        var $boton = $form.find("button[type='submit']");

        $boton.prop("disabled", true);

        $.ajax({
            url: $form.attr("action"),
            type: "POST",
            data: $form.serialize(),
            dataType: "json"
        }).done(function (respuesta) {
            if (!respuesta || !respuesta.ok) {
                mostrarFeedback($feedback, (respuesta && respuesta.message) || "No se pudo guardar el cambio.", "danger");
                return;
            }

            actualizarResumenTicket(respuesta);
            mostrarFeedback($feedback, respuesta.message || "Cambios guardados.", "success");
            ocultarFeedback($feedback, 3000);
        }).fail(function () {
            mostrarFeedback($feedback, "No se pudo guardar el cambio. Intentá nuevamente.", "danger");
        }).always(function () {
            $boton.prop("disabled", false);
        });
    }

    function actualizarResumenTicket(data) {
        if (data.estado) {
            $("#ticketEstadoBadge")
                .text(data.estado)
                .attr("class", "badge fs-6 " + claseEstado(data.estado));
        }

        if (data.idEstado != null) {
            $("#idEstado").val(String(data.idEstado));
        }

        if (data.idUsuarioAsignado != null) {
            $("#idUsuarioAsignado").val(String(data.idUsuarioAsignado));
        } else {
            $("#idUsuarioAsignado").val("");
        }

        $("#asignadoTexto").text(data.asignado || "Sin asignar");

        if (data.fechaActualizacion) {
            $("#fechaActualizacionTexto").text(data.fechaActualizacion);
        }

        var $label = $("#fechaResolucionLabel");
        var $valor = $("#fechaResolucionTexto");

        if (data.fechaResolucion) {
            $label.removeClass("d-none");
            $valor.removeClass("d-none").text(data.fechaResolucion);
        } else {
            $label.addClass("d-none");
            $valor.addClass("d-none").text("");
        }
    }

    function crearComentario(comentario) {
        var $item = $("<div>", { "class": "comment-item" });
        var $meta = $("<div>", { "class": "comment-meta" });

        $("<strong>").text(comentario.usuarioNombre).appendTo($meta);
        $("<span>", { "class": "badge bg-light text-dark border" }).text(comentario.usuarioRol).appendTo($meta);
        $("<span>", { "class": "text-muted small" }).text(comentario.fecha).appendTo($meta);
        $meta.appendTo($item);
        $("<div>", { "class": "comment-text" }).text(comentario.texto).appendTo($item);

        return $item;
    }

    function claseEstado(estado) {
        switch (estado) {
            case "Abierto":
                return "bg-primary";
            case "En Proceso":
                return "bg-warning text-dark";
            case "Resuelto":
                return "bg-success";
            default:
                return "bg-secondary";
        }
    }

    function mostrarFeedback($elemento, mensaje, tipo) {
        clearTimeout($elemento.data("ocultarTimeout"));
        $elemento
            .removeClass("d-none alert-success alert-danger alert-info")
            .addClass("alert alert-" + tipo)
            .text(mensaje);
    }

    function ocultarFeedback($elemento, milisegundos) {
        var timeoutId = setTimeout(function () {
            $elemento.addClass("d-none");
        }, milisegundos);

        $elemento.data("ocultarTimeout", timeoutId);
    }
})(jQuery);
