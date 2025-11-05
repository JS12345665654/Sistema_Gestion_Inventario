// ================== jQuery Validate: mensajes en español ==================
if (window.jQuery && $.validator) {
    $.extend($.validator.messages, {
        required: "Este campo es obligatorio.",
        remote: "Por favor, corrija este campo.",
        email: "Ingrese un correo válido.",
        url: "Ingrese una URL válida.",
        date: "Ingrese una fecha válida.",
        dateISO: "Ingrese una fecha válida (ISO).",
        number: "Ingrese un número válido.",
        digits: "Ingrese solo dígitos.",
        creditcard: "Ingrese un número de tarjeta válido.",
        equalTo: "Repita el mismo valor.",
        extension: "Ingrese un valor con una extensión válida.",
        maxlength: $.validator.format("No ingrese más de {0} caracteres."),
        minlength: $.validator.format("Ingrese al menos {0} caracteres."),
        rangelength: $.validator.format("Ingrese un valor entre {0} y {1} caracteres."),
        range: $.validator.format("Ingrese un valor entre {0} y {1}."),
        max: $.validator.format("Ingrese un valor menor o igual a {0}."),
        min: $.validator.format("Ingrese un valor mayor o igual a {0}.")
    });

    // Aceptar 12,34 y 12.34 como número
    var _number = $.validator.methods.number;
    $.validator.methods.number = function (value, element) {
        if (this.optional(element)) return true;
        var v = (value || "").replace(',', '.');
        return /^-?\d+(\.\d+)?$/.test(v);
    };

    var _range = $.validator.methods.range;
    $.validator.methods.range = function (value, element, param) {
        if (this.optional(element)) return true;
        var v = (value || "").replace(',', '.');
        var num = parseFloat(v);
        return !isNaN(num) && num >= param[0] && num <= param[1];
    };
}

// ================== DataTables: español por defecto ==================
if (window.jQuery && $.fn.dataTable) {
    $.extend(true, $.fn.dataTable.defaults, {
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
        }
    });
}

// ================== SweetAlert helpers en español ==================
window.AppES = (function () {

    async function confirmar(titulo, texto, tipo = 'warning', confirmText = 'Sí, continuar', cancelText = 'Cancelar') {
        const res = await Swal.fire({
            title: titulo,
            text: texto,
            icon: tipo,
            showCancelButton: true,
            confirmButtonText: confirmText,
            cancelButtonText: cancelText,
            reverseButtons: true
        });
        return res.isConfirmed;
    }

    function ok(msg = "Operación realizada.") {
        return Swal.fire({ icon: 'success', title: 'Listo', text: msg });
    }

    function error(msg = "Ocurrió un error.") {
        return Swal.fire({ icon: 'error', title: 'Error', text: msg });
    }

    // Enlace genérico para botones de borrar: .btn-eliminar con data-url y opcional data-nombre
    function bindEliminarConSwal(selectorBtn = '.btn-eliminar', selectorFormHidden = '#formEliminar') {
        document.querySelectorAll(selectorBtn).forEach(btn => {
            btn.addEventListener('click', async function () {
                const url = this.dataset.url;
                const nombre = this.dataset.nombre || 'este registro';
                if (!url) return;

                const okConf = await confirmar('Confirmar eliminación', `¿Eliminar ${nombre}?`, 'warning', 'Eliminar', 'Cancelar');
                if (!okConf) return;

                const form = document.querySelector(selectorFormHidden);
                if (!form) {
                    // fallback si no existe form oculto
                    const f = document.createElement('form');
                    f.method = 'post';
                    f.action = url;
                    // Nota: si tu acción requiere AntiForgery y no hay token, agregá un input con __RequestVerificationToken.
                    document.body.appendChild(f);
                    f.submit();
                    return;
                }
                form.action = url;
                form.submit();
            });
        });
    }

    return { confirmar, ok, error, bindEliminarConSwal };
})();

document.addEventListener('DOMContentLoaded', function () {
    if (window.AppES) {
        AppES.bindEliminarConSwal('.btn-eliminar', '#formEliminar');
    }
});