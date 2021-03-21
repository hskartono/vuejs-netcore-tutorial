<template>
  <b-modal id="modal-input" scrollable :no-close-on-backdrop=true :title="label" :no-close-on-esc=true v-model="show" 
    :ok-title="okTitle"
    ok-variant="success" 
    :cancel-title="cancelTitle" 
    @ok="saveAction"
    @hidden="onHidden">
    <template #modal-header>
      <div class="w-100">
        <b-row>
          <b-col>
            <h5 class="modal-title">{{ label }}</h5>
          </b-col>
          <b-col>
            <span class="float-right">
                <span v-for="(item, index) in editor" :key="item">
                  <b-avatar :variant="getVariant(index)" :id="'editor-avatar-' + index " style="margin-left: -3px" :text="item | short_hand"></b-avatar>
                  <b-tooltip :target="'editor-avatar-' + index ">{{ item }}</b-tooltip>
                </span>
                <button class="close" no-variant @click="$bvModal.hide('modal-input')">×</button>
            </span>
          </b-col>
        </b-row>
      </div>
    </template>
    <template #modal-footer="{ ok, cancel }">
      <slot name="modal-left-button"></slot>
			<b-button size="sm" variant="secondary" class="float-right" @click="cancel()">
				{{ cancelTitle }}
			</b-button>
      <b-button size="sm" variant="success" class="float-right" @click="ok()">
				{{ okTitle }}
			</b-button>
		</template>
    <slot/>
  </b-modal>
</template>

<script>
import axios from 'axios';

export default {
  name: 'PopupWrap',
  data() {
    return {
      show : true,
      editor: [],
    }
  },
  filters: {
   short_hand (name) {
    let words = name.split(" ")
    let short_hand = words[0][0] + (words[words.length-1] != undefined && words[words.length-1] != null && words[words.length-1] != '' ? words[words.length-1][0] : "")
    return short_hand;
   }
  },
  props: {
    centered: {
      default: true,
      type: Boolean,
    },
    focusElement: {
      default: null,
      type: Object,
    },
    label: {
      required: true,
      type: String,
    },
    entityName: {
      type: String,
    },
    documentId: {
      type: String,
    },
    okTitle: {
      default: 'Simpan',
      type: String,
    },
    cancelTitle: {
      default: 'Batal',
      type: String,
    },
  },
  mounted() {
    const close = (e) => {
      const ESC = 27;
      if (e.keyCode !== ESC) return;
      this.$emit('close');
    };

    document.addEventListener('keyup', close);
    this.$on('hook:destroyed', () => {
      document.removeEventListener('keyup', close);
    });

    this.activate();
    this.$on('hook:destroyed', () => {
      this.deactivate();
    });
    
    /*this.$nextTick(() => {
      this.checkEditor();
    });*/
  },
  methods: {
    activate() {
      this.previousActiveElement = document.activeElement;
      this.inert();
    },
    saveAction(modalEvent) {
      this.$emit("saveMethod", modalEvent);
    },
    onHidden() {
      this.$emit('hideMethod');
      this.$emit('close');
    },
    async deactivate() {
      await this.inert(false);
    },
    async inert(status = true) {
      await this.$nextTick();
      [...this.$root.$el.children].forEach((child) => {
        if (child === this.$el || child.contains(this.$el)) return;
        child.inert = status;
      });
    },
    getVariant(index) {
      let variant = ['warning', 'danger', 'info', 'primary', 'success'];
      if (index > (variant.length - 1)) {
        index = index % variant.length;
      }
      return variant[index];
    },
    async checkEditor() {
      if (this.documentId != null && this.documentId != undefined && this.documentId != 'null' && this.documentId != '') {
        this.$http.get(process.env.VUE_APP_API_URL + this.entityName + '/currenteditor/' + this.documentId)
        .then(response => {
          if (response != null) {
              if (response.data.length > 0) {
                this.editor = response.data;
              }
          }
        }).catch(error => {});
      }
    },
  },
  watch : {
    documentId(newValue, oldValue) {
      if (newValue != oldValue)
          this.checkEditor();
    }
  }
};
</script>

<style>
.modal-dialog {
    max-width: 90% !important;
    height: 100vh;
    display: flex;
}
.modal-body {
  background: #EEF0F8 !important;
}
@media (max-width:768px) {
  .modal-dialog {
    max-width: 100% !important;
  } 

  .b-table-top-row {
    background: #f3f6f9 !important;
  }
}
@media (max-width: 576px) {
  .modal-dialog {
    max-width: 100% !important;
  }

  .b-table-top-row {
    background: #f3f6f9 !important;
  }
}
</style>