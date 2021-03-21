<template>
  <Portal to="app-popup">
    <PopupBase
      :label="label"
      :entityName="entityName"
      :documentId="documentId"
      @close="$router.back()"
      @saveMethod="saveAction"
      @hideMethod="onHidden"
      :okTitle="okTitle"
      :cancelTitle="cancelTitle"
    >
      <slot name="backdrop" slot="backdrop"/>
      <slot/>
      <template #modal-left-button>
        <slot name="modal-left-button"></slot>
      </template>
    </PopupBase>
  </Portal>
</template>

<script>
import PopupBase from './PopupBase.vue';

export default {
  name: 'PopupRouterView',
  props: {
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
  methods: {
    saveAction(modalEvent) {
      this.$emit("saveMethod", modalEvent);
    },
    onHidden() {
      this.$emit("onHideMethod");
    }
  },
  components: {
    PopupBase,
  },
};
</script>