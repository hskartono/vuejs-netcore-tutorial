<template>
  <b-modal 
      scrollable 
      :no-close-on-backdrop=true 
      :title="label" 
      :no-close-on-esc=true 
      v-model="show" 
      @cancel="copyDataAction" 
      ok-title="Close" 
      ok-variant="success" 
      @hidden="$emit('close')">
    <div>
      <slot/>
    </div>
    <template #modal-footer="{ ok, cancel }">
      <slot name="modal-left-button"></slot>
			<b-button v-if="showCopyData" size="sm" :variant="cancelVariant" class="float-right" @click="cancel()">
				{{ cancelTitle }}
			</b-button>
      <b-button size="sm" :variant="okVariant" class="float-right" @click="ok()">
				{{ okTitle }}
			</b-button>
		</template>
  </b-modal>
</template>

<script>
export default {
  name: 'PopupWrap',
  //props: ["copyDataMethod", "isShowCopyData", "label"],
  props: {
    copyDataMethod: {},
    isShowCopyData: {
      default: true,
      type: Boolean,
    },
    label: {
      required: true,
      type: String,
    },
    okTitle: {
      default: 'Close',
      type: String,
    },
    cancelTitle: {
      default: 'Copy Data',
      type: String,
    },
    okVariant: {
      default: 'success',
      type: String,
    },
    cancelVariant: {
      default: 'secondary',
      type: String,
    },
  },
  data() {
    return {
      show : true,
    }
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
  },
  computed : {
    showCopyData() {
      if (this.isShowCopyData != null && this.isShowCopyData != undefined) return this.isShowCopyData;
      return true;
    }
  },
  methods: {
    copyDataAction(modalEvent) {
      this.$emit("copyDataMethod", modalEvent)
    },
    activate() {
      this.previousActiveElement = document.activeElement;
      this.inert();
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
  },
};
</script>

<style>
.modal-body {
  background: #EEF0F8 !important;
}
</style>