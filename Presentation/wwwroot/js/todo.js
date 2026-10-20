(function ($, window) {
    "use strict";

    const TodoApp = {
        init: function () {
            this.bindEvents();
            this.loadTodos();
        },

        bindEvents: function () {
            const self = this;

            $("#btn-new").on("click", function () {
                self.openCreateModal();
            });

            $("#todo-save").on("click", function () {
                self.saveTodo();
            });

            $("#todo-modal").on("shown.bs.modal", function () {
                $("#todo-title-input").focus();
            });
        },

        loadTodos: function () {
            const self = this;
            $.get("/Todo/List")
                .done(function (data) {
                    self.render(data);
                })
                .fail(function () {
                    console.error("Error cargando tareas");
                });
        },

        render: function (items) {
            const $list = $("#todo-list");
            $list.empty();

            if (!items || items.length === 0) {
                $list.append('<li class="list-group-item text-muted">Sin tareas</li>');
                return;
            }

            const self = this;
            items.forEach(function (item) {
                const descriptionHtml = item.description
                    ? `<br><span class="ms-4">${self.escapeHtml(item.description)}</span>`
                    : "";
                const html = `
                    <li class="list-group-item" data-id="${self.escapeAttr(item.id)}">
                        <span>${self.escapeHtml(item.title)}</span>${descriptionHtml}
                    </li>`;
                $list.append(html);
            });
        },

        openCreateModal: function () {
            $("#todo-modal-title").text("Nueva tarea");
            $("#todo-title-input").val("");
            $("#todo-description-input").val("");
            $("#todo-modal").modal("show");
        },

        saveTodo: function () {
            const title = $("#todo-title-input").val().trim();
            const description = $("#todo-description-input").val().trim();
            if (!title) {
                return;
            }
            const self = this;
            $.ajax({
                url: "/Todo/Create",
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify({ title: title, description: description })
            }).done(function () {
                $("#todo-modal").modal("hide");
                self.loadTodos();
            }).fail(function () {
                console.error("Error creando tarea");
            });
        },

        escapeHtml: function (text) {
            const div = document.createElement("div");
            div.textContent = text;
            return div.innerHTML;
        },

        escapeAttr: function (text) {
            return String(text).replace(/"/g, "&quot;");
        }
    };

    $(document).ready(function () {
        TodoApp.init();
    });

})(jQuery, window);
